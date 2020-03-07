# creating a new solution file

-> dotnet new sln

# creating a dotnet class library

-> dotnet new classlib -n Domain

# adding reference to base solution file

-> dotnet sln add Domain/

# for adding reference of one project to another

-> cd projectDir<br>
-> dotnet add reference ../Domain/<br>

# for migration

dotnet ef migrations add InitialCreate -p Persistence/ -s API/ <br>
dotnet ef migrations add SeedValue -p Persistence/ -s API/<br>
dotnet ef migrations add "ActivityEntityAdded" -p Persistence -s API/<br>

# for dropping databse

dotnet ef database drop -p Persistence -s API/<br>

# for dotnet tool install

dotnet tool install --global dotnet-ef --version 3.0.0<br>
dotnet tool install --global dotnet-ef --version 3.0.0<br>

# setting secrets in cloudinary

dotnet user-secrets set "Cloudinary:CloudName" "duq38xmx7"<br>
dotnet user-secrets set "Cloudinary:ApiKey" "duq38xmx7"<br>
dotnet user-secrets init -p API/ <br>

# for publishing

dotnet publish -c Release -o publish --self-contained false Reactivities.sln <br>
dotnet publish -c Release -o publish --self-contained true Reactivities.sln -r osx-x64
