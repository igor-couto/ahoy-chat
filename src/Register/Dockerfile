FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /app
COPY ./src/Register/*.csproj Register/
COPY ./src/Shared/*.csproj Shared/
RUN dotnet restore "Register/AhoyRegister.csproj" --disable-parallel --runtime linux-musl-x64
COPY . ./
RUN dotnet publish ./src/Register -c Release -o out --runtime linux-musl-x64 --self-contained true
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
# ENV ASPNETCORE_URLS=https://+:8083;http://+:8082
ENV ASPNETCORE_URLS=http://+:8082
ENV ASPNETCORE_HTTP_PORT=8082
# ENV ASPNETCORE_HTTPs_PORT=8083
EXPOSE 8082
# EXPOSE 8083
ENTRYPOINT ["./AhoyRegister"]

# This works: 235.17 MB
# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
# WORKDIR /app
# COPY ./src/Register/*.csproj Register/
# COPY ./src/Shared/*.csproj Shared/
# RUN dotnet restore "Register/AhoyRegister.csproj" --disable-parallel 
# COPY . ./
# RUN dotnet publish ./src/Register -c Release -o out

# FROM mcr.microsoft.com/dotnet/aspnet:7.0
# WORKDIR /app
# COPY --from=build-env /app/out .
# RUN ls -a
# # ENV ASPNETCORE_URLS=https://+:8083;http://+:8082
# ENV ASPNETCORE_URLS=http://+:8082
# ENV ASPNETCORE_HTTP_PORT=8082
# # ENV ASPNETCORE_HTTPs_PORT=8083
# EXPOSE 8082
# # EXPOSE 8083
# ENTRYPOINT ["dotnet", "AhoyRegister.dll"]