FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ToDo application/ToDo application.csproj", "ToDo application/"]
RUN dotnet restore "ToDo application/ToDo application.csproj"
COPY . .
WORKDIR "/src/ToDo application"
RUN dotnet build "ToDo application.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ToDo application.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDo application.dll"]
