# BlazorWebV1

개인 개발자 및 컴퓨터공학 전공자의 학습 기록용 기술 블로그입니다.

## Stack

- C# / ASP.NET Core Blazor Server
- ASP.NET Core Identity
- Entity Framework Core
- MS SQL Server LocalDB

## 주요 기능

- 다크 테마 기반 기술 블로그 UI
- 카테고리 탐색, 검색, 태그, 관련 글
- 게시글 상세 자동 목차
- 코드 블록 복사 버튼과 줄바꿈 처리
- 이메일 인증 회원가입
- 아이디 또는 이메일 로그인
- 관리자 전용 게시글 작성/수정 화면
- 관리자 전용 카테고리 관리 화면

## 개발 실행

```powershell
dotnet restore
dotnet ef database update
dotnet run
```

## 관리자 계정 설정

`appsettings.Development.json`은 Git에 포함하지 않습니다. 처음 실행 전 `appsettings.Development.example.json`을 복사해서 비밀번호를 변경하거나, 사용자 시크릿으로 설정하세요.

```powershell
dotnet user-secrets set "SeedAdmin:Email" "admin@example.com"
dotnet user-secrets set "SeedAdmin:UserName" "admin"
dotnet user-secrets set "SeedAdmin:Password" "ChangeThisPassword123!"
dotnet user-secrets set "SeedAdmin:Nickname" "Archive Admin"
```

SMTP가 설정되지 않으면 이메일 인증/비밀번호 재설정 링크는 로그에만 기록됩니다.
