FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /app
COPY ["./Chat/AhoyChat.csproj", "AhoyChat/"]
COPY ["./Shared/AhoyShared.csproj", "Shared/"]
RUN dotnet restore "Chat/AhoyChat.csproj" --disable-parallel --runtime linux-musl-x64
COPY . .
RUN dotnet publish -c Release -o out --runtime linux-musl-x64 --self-contained true

# Generate the self-signed certificate
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
ENV ASPNETCORE_URLS=https://+:50445;http://+:50446
ENV ASPNETCORE_HTTP_PORT=50446
ENV ASPNETCORE_HTTPs_PORT=50445
EXPOSE 5127
EXPOSE 7290
ENTRYPOINT ["./AhoyChat"]
