#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["sample/grpc.net/Overt.GrpcExample.Service/Overt.GrpcExample.Service.csproj", "sample/grpc.net/Overt.GrpcExample.Service/"]
RUN dotnet restore "sample/grpc.net/Overt.GrpcExample.Service/Overt.GrpcExample.Service.csproj"
COPY . .
WORKDIR "/src/sample/grpc.net/Overt.GrpcExample.Service"
RUN dotnet build "Overt.GrpcExample.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Overt.GrpcExample.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Overt.GrpcExample.Service.dll"]