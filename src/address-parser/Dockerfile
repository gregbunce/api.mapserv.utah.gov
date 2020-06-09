FROM python:3-buster AS base
EXPOSE 8000
WORKDIR /usr/src/app
RUN pip install uvicorn blacksheep usaddress agrc-sweeper

COPY server.py .

ENTRYPOINT [ "uvicorn", "server:app", "--host", "0.0.0.0" ]
