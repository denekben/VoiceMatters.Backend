FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app
COPY ["VoiceMatters.WebUI/VoiceMatters.WebUI.csproj", "VoiceMatters.WebUI/"]
COPY ["VoiceMatters.Infrastructure/VoiceMatters.Infrastructure.csproj", "VoiceMatters.Infrastructure/"]
COPY ["VoiceMatters.Application/VoiceMatters.Application.csproj", "VoiceMatters.Application/"]
COPY ["VoiceMatters.Domain/VoiceMatters.Domain.csproj", "VoiceMatters.Domain/"]
COPY ["VoiceMatters.Shared/VoiceMatters.Shared.csproj", "VoiceMatters.Shared/"]
RUN dotnet restore "VoiceMatters.WebUI/VoiceMatters.WebUI.csproj"
COPY . ./
RUN dotnet publish "VoiceMatters.WebUI/VoiceMatters.WebUI.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
ENV ASPNETCORE_URLS=http://0.0.0.0:80  
ENTRYPOINT [ "dotnet", "VoiceMatters.WebUI.dll" ]