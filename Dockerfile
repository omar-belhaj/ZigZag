FROM python:3.11

WORKDIR /app

ENV PYTHONUNBUFFERED=True

COPY requirements.txt /requirements.txt

RUN pip install --no-cache-dir --upgrade -r /requirements.txt

ARG PORT=8501
ENV PORT=${PORT}

EXPOSE ${PORT}

COPY . ./app

COPY ./data /app/data

ENTRYPOINT streamlit run --server.port $PORT app/app.py --server.address=0.0.0.0