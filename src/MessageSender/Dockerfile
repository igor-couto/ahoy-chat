FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /app
COPY ["./MessageSender/AhoyMessageSender.csproj", "MessageSender/"]
COPY ["./Shared/AhoyShared.csproj", "Shared/"]
RUN dotnet restore "MessageSender/AhoyMessageSender.csproj" --disable-parallel --runtime linux-musl-x64
COPY . .
RUN dotnet publish -c Release -o out --runtime linux-musl-x64 --self-contained true

FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine
RUN apk upgrade musl
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app
USER dotnetuser
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["./MessageSender"]