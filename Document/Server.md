## NuGet List      

## 서버 설정
`appsettings.json` 파일에 있다.  
  

## Thread 구성
MQ 네트워크 스레드는 제외  
  
- 네트워크 스레드
    - 개수: .NET의 Thread Pool
    - SuperSocket 내부에서 사용
    - 클라이언트의 요청을 처리
- Packet 처리 스레드
    - 개수: 1
    - 서버에서 동기화 없이 일괄처리하기 위한 스레드
 
![](./images/1.png)      