@rem Copyright 2016 gRPC authors.
@rem
@rem Licensed under the Apache License, Version 2.0 (the "License");
@rem you may not use this file except in compliance with the License.
@rem You may obtain a copy of the License at
@rem
@rem     http://www.apache.org/licenses/LICENSE-2.0
@rem
@rem Unless required by applicable law or agreed to in writing, software
@rem distributed under the License is distributed on an "AS IS" BASIS,
@rem WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
@rem See the License for the specific language governing permissions and
@rem limitations under the License.

@rem Generate the C# code for .proto files

setlocal

@rem enter this directory
cd /d %~dp0

set TOOLS_PATH=C:\Users\Administrator\.nuget\packages\grpc.tools\1.7.1\tools\windows_x64

%TOOLS_PATH%\protoc.exe --csharp_out "src/2. DotNetCore/Sodao.GrpcService.Library/Generate" "src/2. DotNetCore/Sodao.GrpcService.Library/GrpcService.proto" --grpc_out "src/2. DotNetCore/Sodao.GrpcService.Library/Generate" --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

endlocal
