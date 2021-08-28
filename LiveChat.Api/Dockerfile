#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["LiveChat.Api/LiveChat.Api.csproj", "LiveChat.Api/"]
COPY ["LiveChat.Business/LiveChat.Business.csproj", "LiveChat.Business/"]
COPY ["LiveChat.Constants/LiveChat.Constants.csproj", "LiveChat.Constants/"]
COPY ["LiveChat.Data/LiveChat.Data.csproj", "LiveChat.Data/"]
RUN dotnet restore "LiveChat.Api/LiveChat.Api.csproj"
COPY . .
WORKDIR "/src/LiveChat.Api"
RUN dotnet build "LiveChat.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LiveChat.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LiveChat.Api.dll"]