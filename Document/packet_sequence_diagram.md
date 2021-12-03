# 패킷 시퀸스 다이얼그램
  
## API Server      
  
### 계정 생성  
```
@startuml
Client <-> APIServer: Req/ResCreate
note right APIServer: DB에 중복 ID를 체크한다.
@enduml
```
  
### 로그인  
```
@startuml
Client <-> APIServer: Req/ResLogin
note right APIServer: Redis 중복 로그인 검증, 보안토큰 생성 및 전달
@enduml
```
  
### 유저 정보 받기
```
@startuml
Client <-> APIServer: Req/ResUserInfo
note right APIServer: 유저 개인의 정보를 준다. ex) 경험치, 몬스터, 소지품 등등

@enduml
```


### 로봇몬 필드 생성
```
@startuml
Client <-> APIServer: Req/ResGetField
note right APIServer: 유저 위치 정보에 따른 주변 지역 정보를 제공한다. 
@enduml
```
 
### 유저 이동
```
@startuml
Client <-> APIServer: Req/ResMove
note right APIServer: 유저 이동한 위치 정보에 따른 주변 지역 정보를 제공한다. (기존 위치의 정보를 제외한 정보) 
@enduml
```
 
### 로봇몬 잡기
```
@startuml
Client <-> APIServer: Req/ResMove
note right APIServer: 유저 이동한 위치 정보에 따른 주변 지역 정보를 제공한다. (기존 위치의 정보를 제외한 정보) 
@enduml
```

### 출석부
```
@startuml
Client <-> APIServer: Req/ResDailyCheck
note right APIServer: 출석체크에 따른 보상을 제공한다. 
@enduml
```

### 친구추가
```
@startuml
Client <-> APIServer: Req/ResAddFriend
note right APIServer: 친구가 존재한다면 친구 등록을 진행한다.
@enduml
``` 

### 랭킹 확인
```
@startuml
Client <-> APIServer: Req/ResCheckRank
note right APIServer: .
@enduml
``` 

### 선물 보내기
```
@startuml
Client <-> APIServer: Req/ResSendPresent
note right APIServer: .
@enduml
``` 

### 선물 받기
```
@startuml
Client <-> APIServer: Req/ResGetPresent
note right APIServer: .
@enduml
``` 

## PVP Server
  
### 필드 입장 - 아직 수정하지 않음
```
@startuml
participant Client
participant APIServer
participant Redis
participant GatewayServer
participant GameServer
participant DBServer

Client -> APIServer : http://*/EnterField
APIServer -> Redis : 중복접속 확인 / 토큰 저장
APIServer -> Client : 중복접속 확인 응답
Client -> GatewayServer: 소켓 접속
Client -> GatewayServer : PkGWSLoginReq
GatewayServer -> DBServer : PkDBSCheckLoginReq
DBServer <-> Redis : 인증 정보 검증
GatewayServer <- DBServer : PkDBSCheckLoginRes
GatewayServer -> Client : PkGWSLoginRes
Client -> GatewayServer : PkGWSEnterFieldReq
GatewayServer -> GameServer : PkGSEnterFieldReq
GameServer -> DBServer : PkDBSLoadCharacterInfoReq
GameServer <- DBServer : PkDBSLoadCharacterInfoRes
GameServer -> GatewayServer : PkGSEnterFieldRes
GatewayServer -> Client : PkGWSEnterFieldRes
@enduml
```  

#### 게임서버 접속  - 아직 수정하지 않음
```
@startuml
participant Client
participant GatewayServer
participant GameServer 

Client -> GatewayServer: ReqGWSFieldEnter
GatewayServer -> GameServer: ReqFieldEnter
GatewayServer <- GameServer: ResFieldEnter
Client <- GatewayServer: ResGWSFieldEnter
@enduml
```


## Raid Server