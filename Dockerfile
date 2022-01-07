FROM rockylinux

RUN dnf update -y
RUN dnf install sudo -y
RUN dnf install vim -y
RUN dnf install redis -y
RUN dnf install mysql-server -y
RUN dnf install procps -y
RUN dnf install dotnet -y

EXPOSE 5000

COPY ./APIServer/bin/Release ./home