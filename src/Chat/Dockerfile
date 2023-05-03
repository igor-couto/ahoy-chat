FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /app
COPY ./src/Chat/*.csproj Chat/
COPY ./src/Shared/*.csproj Shared/
RUN dotnet restore "Chat/AhoyChat.csproj" --disable-parallel --runtime linux-musl-x64
COPY . ./
RUN dotnet publish ./src/Chat -c Release -o out --runtime linux-musl-x64 --self-contained true
RUN apk add --no-cache openssl
RUN mkdir /tmp/certs && \
    openssl req -x509 -newkey rsa:4096 -keyout /tmp/certs/certificate.key -out /tmp/certs/certificate.crt -days 365 -nodes -subj "/CN=localhost"

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine
RUN apk upgrade musl
RUN apk add --no-cache openssl
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
WORKDIR /app
COPY --from=build /app/out .
COPY --from=build /tmp/certs ./certs
# ENV ASPNETCORE_URLS=https://+:8087;http://+:8084
ENV ASPNETCORE_URLS=http://+:8086
ENV ASPNETCORE_HTTP_PORT=8086
# ENV ASPNETCORE_HTTPs_PORT=8087
EXPOSE 8086
# EXPOSE 8087
ENTRYPOINT ["./AhoyChat"]