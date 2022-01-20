# 테스트 환경 구조
[그림 추가 예정]

# docker 설치, 켜기
sudo wget -qO- http://get.docker.com/ | sh  
sudo systemctl start docker  

# 파일 다운 받기
sudo docker pull richard0326/robotmon-go-apiserver  
sudo docker pull richard0326/fluentd  

# 사전 작업
mysql dump 파일을 미리 세팅해둔다.
