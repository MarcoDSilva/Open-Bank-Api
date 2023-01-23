
# Open Bank

An Bank Web API following Clean Code + Domain Driven Design. 


[![My Tech Stack](https://github-readme-tech-stack.vercel.app/api/cards?title=Tech%20Stack&lineCount=3&gap=12&line1=.net,.net,512BD4;csharp,csharp,239120;&line2=JSON%20Web%20Tokens,JSON%20Web%20Tokens,000000;Postman,Postman,FF6C37;JSON,JSON,000000;&line3=Docker,Docker,2496ED;Apache%20Kafka,Apache%20Kafka,231F20;Postgresql,PostgreSql,4169E1;)](https://github-readme-tech-stack.vercel.app/api/cards?title=Tech%20Stack&lineCount=3&gap=12&line1=.net,.net,512BD4;csharp,csharp,239120;&line2=JSON%20Web%20Tokens,JSON%20Web%20Tokens,000000;Postman,Postman,FF6C37;JSON,JSON,000000;&line3=Docker,Docker,2496ED;Apache%20Kafka,Apache%20Kafka,231F20;Postgresql,PostgreSql,4169E1;)


## Run Locally

Clone the project

```bash
  git clone https://github.com/MarcoDSilva/Open-Bank-Api.git
```

Run the docker composer (Sidenote: uncomment the postgresql lines and associate the ports with the ones from the connection string , or the ports you prefer)
```bash
  docker compose up
```

Go to the project directory

```bash
  cd Open-Bank-Api
```

Create the database (if the postgresql container is not running this will fail)

```bash
  dotnet ef database update
```

Start the server

```bash
  cd .\OpenBank.API\
  dotnet run
```

