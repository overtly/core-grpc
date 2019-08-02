FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY src/Overt.GrpcExampleCrossPlat.Service/Overt.GrpcExampleCrossPlat.Service.csproj src/Overt.GrpcExampleCrossPlat.Service/
COPY src/Overt.GrpcExample.Application/Overt.GrpcExample.Application.csproj src/Overt.GrpcExample.Application/
COPY src/Overt.GrpcExample.Domain/Overt.GrpcExample.Domain.csproj src/Overt.GrpcExample.Domain/
COPY src/Overt.GrpcExampleCrossPlat.Service.Grpc/Overt.GrpcExampleCrossPlat.Service.Grpc.csproj src/Overt.GrpcExampleCrossPlat.Service.Grpc/
COPY src/Overt.Core.Grpc/Overt.Core.Grpc.csproj src/Overt.Core.Grpc/
RUN dotnet restore
COPY . .
WORKDIR /src/src/Overt.GrpcExampleCrossPlat.Service
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Overt.GrpcExampleCrossPlat.Service.dll"]
