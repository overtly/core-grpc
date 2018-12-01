FROM microsoft/dotnet:2.0-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /src
COPY *.sln ./
COPY src/Sodao.GrpcExample.Service/Sodao.GrpcExample.Service.csproj src/Sodao.GrpcExample.Service/
COPY src/Sodao.GrpcExample.Application/Sodao.GrpcExample.Application.csproj src/Sodao.GrpcExample.Application/
COPY src/Sodao.GrpcExample.Domain/Sodao.GrpcExample.Domain.csproj src/Sodao.GrpcExample.Domain/
COPY src/Sodao.GrpcExample.Service.Grpc/Sodao.GrpcExample.Service.Grpc.csproj src/Sodao.GrpcExample.Service.Grpc/
COPY src/Sodao.Core.Grpc/Sodao.Core.Grpc.csproj src/Sodao.Core.Grpc/
RUN dotnet restore
COPY . .
WORKDIR /src/src/Sodao.GrpcExample.Service
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Sodao.GrpcExample.Service.dll"]
