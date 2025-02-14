# ZigZag : Application éducative basée sur l'IA générative

ZigZag est une application innovante combinant intelligence artificielle et éducation. Elle propose une double interface :

- **Pour les enfants** : Des jeux interactifs et adaptés à leur âge pour stimuler l’apprentissage tout en identifiant leurs forces et faiblesses (difficultés d’apprentissage, dyslexie, etc.).
- **Pour les éducateurs** : Un assistant virtuel dédié qui analyse les données des enfants et fournit des conseils personnalisés ainsi que des méthodes pédagogiques optimisées.

## 🌟 Objectifs du projet

### Aider les enfants
- Offrir des contenus éducatifs adaptés à chaque enfant.
- Respecter les normes de temps d’écran recommandées par l’OMS (30 minutes/jour maximum).

### Soutenir les éducateurs
- Fournir des conseils pédagogiques personnalisés basés sur les données collectées.
- Intégrer des documents pédagogiques adaptés.

### Gagner en efficacité
- Réduire la charge de travail des éducateurs.
- Optimiser les ressources dans les crèches et écoles.

## 🔀 Fonctionnalités

### Pour les enfants :
- **Jeux éducatifs personnalisés** :
  - *Wordsearch* : Recherche de mots adaptée au niveau de difficulté.
  - *Équations mathématiques* : Générées dynamiquement pour s’adapter à chaque enfant.
- **Suivi et analyse** :
  - Collecte de données sur les performances pour identifier les problèmes d’apprentissage.

### Pour les éducateurs :
- **Chatbot pédagogique** :
  - Conseils sur les méthodes d’enseignement adaptées à chaque enfant.
  - Analyse des progrès et des besoins individuels.
- **Rapports détaillés** :
  - Visualisation des données des enfants pour un meilleur suivi.

## 💻 Technologies utilisées

### Frontend :
- Unity pour l’interface des jeux éducatifs.

### Backend :
- API OpenAI (GPT-4 pour le chatbot, DALL-E pour la génération d’images).
- ASP.NET Core pour la gestion des données et des interactions.
- SocketIO.
- Docker.

## 📦 Installation et exécution

### Prérequis :
- .NET SDK 6.0 ou supérieur.
- Unity installé sur votre machine.
- Clé API OpenAI (à renseigner dans un fichier `.env`).

### Étapes :

1. Clonez le projet :
   ```bash
   git clone https://github.com/omar-belhaj/CoddsCoders.git
   cd CoddsCoders
   ```
2. Configurez la clé API OpenAI :
   - Créez un fichier `.env` à la racine et ajoutez :
     ```
     OPENAI_API_KEY=your-api-key
     ```

## 🚀 Déploiement avec Docker

### 1. Construire et Déployer une Image Docker pour l'Application ZigZag
L'objectif est de créer une interface Streamlit du chatbot pour l'application ZigZag et de la déployer sur Google Cloud Platform (GCP).

### 1.1 Tests Locaux

#### **A. Créer et Tester l'Application Streamlit Localement**
1. Créez un fichier `app.py` contenant le code de l'application Streamlit.
2. Lancez l'application localement :
   ```bash
   streamlit run app.py
   ```

#### **B. Construire l'Image Docker**
1. Assurez-vous que le `Dockerfile` est bien configuré.
2. Construisez une image Docker :
   ```bash
   docker build -t streamlit:zigzag .
   docker run --name zigzag_chatbot -p 8502:8501 streamlit:zigzag
   ```

3. Pour arrêter et relancer le conteneur :
   ```bash
   docker stop zigzag_chatbot
   docker rm zigzag_chatbot
   docker run -p 8502:8501 streamlit:zigzag
   ```

## 📊 Business Model

L’application fonctionne sur un modèle d’abonnement mensuel :
- **Pour les crèches et écoles** : Facturation basée sur le nombre d’enfants et d’éducateurs.
- **Avantages pour les établissements** :
  - Gain de temps et d’efficacité pour les éducateurs.
  - Meilleur suivi des enfants en difficulté.

## 🛠 Contributions

Les contributions sont les bienvenues ! Veuillez ouvrir une *issue* ou soumettre une *pull request* pour toute amélioration ou suggestion.

## 🌐 Contact

Pour toute question ou demande de partenariat, contactez-nous :

- 📧 Email :
  - gatti.aziz55@gmail.com
  - youssefeloued789@gmail.com
  - mohamedkhaled.dridi2@gmail.com
  - rodrigue.migniha@dauphine.tn
  - adamfatnassi110@gmail.com
  - omarbelhadj220@gmail.com
- 🌍 GitHub : [ZigZag](https://github.com/omar-belhaj/CoddsCoders)
