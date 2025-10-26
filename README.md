docker run -d \
--name YestinoDb \
-e POSTGRES_USER=postgres \
-e POSTGRES_PASSWORD=postgres \
-e POSTGRES_DB=YestinoDb \
-p 1234:5432 \
postgres:latest
