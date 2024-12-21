Projet Éducatif pour Enfants (4-8 ans) 🧒🎓

Ce projet vise à développer une application éducative interactive pour les enfants âgés de 4 à 8 ans, en combinant technologies modernes et intelligence artificielle. L'objectif principal est de générer des exercices adaptés au niveau de chaque enfant, en s'appuyant sur leurs performances passées, afin de stimuler leur apprentissage dans des domaines variés comme les mathématiques , et les langues française et anglaise.

🌟 Fonctionnalités
Génération Dynamique de Questions : Utilisation de l'API OpenAI pour produire des questions personnalisées basées sur l'âge, les résultats passés et le niveau de performance.
Explications Adaptées aux Enfants : Fournir des explications simples et conviviales pour faciliter l'apprentissage.
Sauvegarde des Performances : Stockage des données des utilisateurs localement pour ajuster les questions aux progrès de chaque enfant.
Chatbot Intelligent : Intégration d'un chatbot interactif :
Pour les enfants : Répond aux questions, propose des conseils et motive les utilisateurs à progresser dans leur apprentissage.
Pour les enseignants : Fournit une assistance pédagogique, génère des suggestions pour améliorer les performances des élèves et propose des rapports détaillés.
🔧 Technologies Utilisées
Voici les technologies que nous avons intégrées pour le développement :

Unity : Moteur de jeu utilisé pour créer l'interface utilisateur et les interactions.
Python & Jupyter : Création du ChatBot.
Docker : Conteneurisation pour simplifier le déploiement et la gestion des dépendances.
API OpenAI : Génération de contenu éducatif interactif en utilisant GPT-4 pour les questions et le chatbot.
🚀 Installation et Exécution
Prérequis
Docker installé sur votre machine.
Unity configuré pour exécuter le projet.
Une clé API valide pour OpenAI (stockée dans un fichier .env).
Étapes :
Clonez ce dépôt :

bash
Copier le code
git clone https://github.com/votre-utilisateur/nom-du-projet.git  
cd nom-du-projet  
Configurez vos clés API dans un fichier .env :

plaintext
Copier le code
OPENAI_API_KEY=VotreCléAPI  
Lancez le conteneur Docker :

bash
Copier le code
docker-compose up  
Importez le projet dans Unity et lancez-le pour tester l'application.

💬 Fonctionnement du Chatbot
Accès pour les enfants : Depuis l'application Unity, les enfants peuvent poser des questions simples ou demander des conseils directement au chatbot, qui répond dans un langage adapté à leur âge.
Accès pour les enseignants : Une interface dédiée permet aux enseignants d'interagir avec le chatbot pour analyser les performances des enfants et recevoir des recommandations éducatives.
💡 Auteurs
[Nom de l'équipe ou des contributeurs]
📄 Licence
Ce projet est sous licence [Nom de la Licence]. Consultez le fichier LICENSE pour plus d'informations.
