import streamlit as st
import os
import requests
from dotenv import load_dotenv
from langchain.llms import OpenAI
from langchain.prompts import ChatPromptTemplate
from langchain.chains import RetrievalQA
from langchain.vectorstores import FAISS
from langchain.embeddings.openai import OpenAIEmbeddings
from langchain.document_loaders import TextLoader
from langchain.text_splitter import RecursiveCharacterTextSplitter
from langchain_openai import ChatOpenAI
from typing import List
import wikipediaapi

# Charger les variables d'environnement
load_dotenv()
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")

# Charger et indexer les documents avec un cache pour améliorer la performance
@st.cache_resource
def load_and_process_documents(filename: str):
    loader = TextLoader(filename, encoding='utf-8')
    documents = loader.load()

    text_splitter = RecursiveCharacterTextSplitter(chunk_size=500, chunk_overlap=50)
    docs = text_splitter.split_documents(documents)

    embeddings = OpenAIEmbeddings(api_key=OPENAI_API_KEY)
    vectorstore = FAISS.from_documents(docs, embeddings)
    return vectorstore

# Fonction de recherche externe sur Wikipedia
@st.cache_data
def search_external_knowledge(query: str) -> str:
    user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
    query = query.replace(" ", "_")
    url = f"https://fr.wikipedia.org/w/api.php?action=query&format=json&prop=extracts&exintro=&titles={query}"
    headers = {'User-Agent': user_agent}
    
    response = requests.get(url, headers=headers)
    
    if response.status_code == 200:
        data = response.json()
        pages = data.get('query', {}).get('pages', {})
        
        if pages:
            page_id = next(iter(pages))
            page = pages[page_id]
            if 'extract' in page:
                return page['extract'][:1000]  # Retourne les 1000 premiers caractères
        return "Je n'ai pas trouvé d'information sur cette page."
    else:
        return "Désolé, il y a eu un problème pour chercher sur Wikipedia."

# Formater les documents extraits pour l'affichage dans le prompt
def format_docs(docs: List):
    return "\n\n".join(doc.page_content for doc in docs)

# Fonction principale pour interroger le modèle GPT-4
def query_with_external_knowledge(query: str, top_k: int = 3, temperature=0.0, max_tokens=150):
    vectorstore = load_and_process_documents("../data/contes_math_jeux.txt")
    docs_relevant = vectorstore.similarity_search(query, k=top_k)
    external_info = search_external_knowledge(query)

    prompt = ChatPromptTemplate.from_messages([
        ("system", "Tu es un assistant super sympa qui aide les enfants à comprendre des choses et répondre à leurs questions. Tu utilises des informations provenant de livres et d'Internet pour donner les meilleures réponses."),
        ("user", f"Voici quelques informations qui pourraient t'aider : {format_docs(docs_relevant)}\n\nEt voici ce que j'ai trouvé sur Internet : {external_info}\n\nTa question est : {query}")
    ])

    formatted_prompt = prompt.format_messages(query=query)

    llm = ChatOpenAI(
        model="gpt-4", 
        temperature=temperature,
        max_tokens=max_tokens,  # Utilisation du max_tokens configuré par l'utilisateur
        api_key=OPENAI_API_KEY
    )

    ai_msg = llm.invoke(formatted_prompt)
    return ai_msg.content if hasattr(ai_msg, 'content') else "Désolé, je n'ai pas pu trouver une réponse."

# Interface utilisateur avec gestion de l'état
def main():
    st.title("Super Assistant pour les Enfants")

    # Contrôles pour la température et le top_k via la sidebar
    temperature = st.sidebar.slider("Température (pour varier les réponses)", 0.0, 1.0, 0.7)
    top_k = st.sidebar.slider("Top K (résultats)", 1, 10, 3)

    # Slider pour le maximum de tokens
    max_tokens = st.sidebar.slider("Nombre maximum de tokens pour la réponse", 50, 500, 150)

    # Initialiser l'historique des messages dans session_state
    if "messages" not in st.session_state:
        st.session_state.messages = []

    # Zone de texte pour la question de l'utilisateur
    user_input = st.text_input("Pose ta question ici 👇")

    if user_input:
        # Ajouter la question de l'utilisateur dans l'historique
        st.session_state.messages.append({"role": "user", "content": user_input})

        # Lancer la génération de la réponse
        with st.spinner("Je réfléchis à ta question..."):
            result = query_with_external_knowledge(user_input, top_k=top_k, temperature=temperature, max_tokens=max_tokens)

        # Ajouter la réponse de l'assistant à l'historique
        st.session_state.messages.append({"role": "assistant", "content": result})

        # Afficher l'historique des messages
        for message in st.session_state.messages:
            if message["role"] == "user":
                st.write(f"**Toi :** {message['content']}")
            else:
                st.write(f"**Super Assistant :** {message['content']}")

if __name__ == "__main__":
    main()
