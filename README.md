
## 1. Build and Deploy a Docker Image for a Streamlit App

The goal of this session is to create a "Hello World" Streamlit interface that will be deployed on GCP.
The first step is to create a Docker image that will contain the Streamlit application. You need to have Docker installed on your machine.

### 1.1 Local Testing

**A. Create and Test the Streamlit App Locally**



```bash
streamlit run app.py
# Open the URL given in localhost
```

**D. Build the Docker Image**

Open and edit the `Dockerfile` as required to match the port exposed below. We create a Docker image that will contain the Streamlit app. The Dockerfile is already created in the root folder. Refer to the `docker build` and `docker run` documentation. We use Docker because it is mandatory to deploy an app on GCP.

```bash
docker build -t streamlit:educ .
docker run --name my_container -p 8502:8501 streamlit:educ
# Open the URL given
```

Once it works, you can use the following commands:

```bash
docker stop my_container
docker rm <my_container>
# Then you can rerun docker run -p 8080:8080 streamlit:latest without any problems
# If you have an "already in use" error, do the previous steps before rerunning
```

