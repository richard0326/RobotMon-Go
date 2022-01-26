# 패킷 시퀸스 다이얼그램
  
## API Server      
  
### 계정 생성  
```
@startuml
Client <-> APIServer: Req/Res CreateAccountController
note right APIServer: DB에 중복 ID를 체크한다.
@enduml
```
  
### 로그인  
```
@startuml
Client <-> APIServer: Req/Res LoginController
note right APIServer: Redis 중복 로그인 검증, 보안토큰 생성 및 전달
@enduml
```
  
### 유저 정보 받기
```
@startuml
Client <-> APIServer: Req/Res UserGameInfoController
note right APIServer: 유저 개인의 정보를 준다. ex) 경험치, 몬스터, 소지품 등등

@enduml
```


### 로봇몬 필드 정보 받기
```
@startuml
Client <-> APIServer: Req/Res FieldMonsterController
note right APIServer: 유저 위치 정보에 따른 주변 지역 정보를 제공한다. 
@enduml
```
 
### 로봇몬 잡기
```
@startuml
Client <-> APIServer: Req/Res CatchController
note right APIServer: 유저 이동한 위치 정보에 따른 주변 지역 정보를 제공한다. (기존 위치의 정보를 제외한 정보) 
@enduml
```

### 로봇몬 연구소 보내기
```
@startuml
Client <-> APIServer: Req/Res RemoveCatchController
note right APIServer: 유저 이동한 위치 정보에 따른 주변 지역 정보를 제공한다. (기존 위치의 정보를 제외한 정보) 
@enduml
```

### 로봇몬 잡은 목룍 보기
```
@startuml
Client <-> APIServer: Req/Res CatchListController
@enduml
```

### 출석체크
```
@startuml
Client <-> APIServer: Req/Res DailyCheckController
note right APIServer: 출석체크에 따른 보상을 제공한다. 
@enduml
```

### 랭킹 확인
```
@startuml
Client <-> APIServer: Req/Res RankingListController
note right APIServer: .
@enduml
``` 

### 선물 보내기
```
@startuml
Client <-> APIServer: Req/Res SendPresentController
note right APIServer: .
@enduml
``` 

### 선물 목록 보기
```
@startuml
Client <-> APIServer: Req/Res MailListController
note right APIServer: .
@enduml
``` 

### 선물 주기
```
@startuml
Client <-> APIServer: Req/Res SendMailController
note right APIServer: .
@enduml
``` 

### 선물 받기
```
@startuml
Client <-> APIServer: Req/Re sRecvMailController
note right APIServer: .
@enduml
``` 

### 강화
```
@startuml
Client <-> APIServer: Req/Res UpgradeController
note right APIServer: .
@enduml
``` 

### 진화
```
@startuml
Client <-> APIServer: Req/Res EvolveController
note right APIServer: .
@enduml
``` 
