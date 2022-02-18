## Vehicel Products 

<p>
This project is using Asp.Net Core 6.0 using MVC and MVVM design patterns. This will include all CRUD operations 
using Entity Frameworks. 
</p>

### Main Web Project includes:
<ol>
	<li>CRUD Operations</li>
	<li>Authorizations (Admin and Customer) roles</li>
</ol>

#### VehicleProducts_xUnitTests Test Project includes:
<ol>
	<li>Constructor Tests</li>
	<li>Data Access Layer Tests</li>
</ol>

<p>
If you prefered antoher flavour in PHP using WordPress Them and Plugin developments. Please visit them Following links. 

<a href="https://www.youtube.com/watch?v=vj1Nqwbe0WI&list=PLDmut58RVgN4UBhUwrW6fQohN4MAhDTlM">WordPress Theme and Plugin tutorials on YouTube</a> <br />
<a href="https://github.com/freeburma/mythemecustomtable">WordPress Theme Development Source Code </a> <br />
<a href="https://github.com/freeburma/product_custom_table">WordPress Plugin Development Source Code </a> <br />
</p>

#### Expected your skill  
1. Routing 
2. Visual Studio or VisualStudio Code with Command Line
3. Entity Framework 

### How to run after brach 3-Admin-Controller-01: 
#### Command Line

<h3>First Time Running</h3>

<ol>
<li> Installing Required Dependencies 
	<ul>
		<li>$ dotnet tool install --global dotnet-ef </li>
		<li>$ dotnet tool update --global dotnet-ef</li>
	</ul>
</li>
	
<li> Creating Db and Tables
	<ul>
		<li>$ dotnet ef migrations add "preparation"</li>
		<li>$ dotnet ef database update</li>
		<li></li>
	</ul>
</li>

<li> Running the website
	<ul>
		<li>$ dotnet watch run or Press <b> F5 </b> or <b>Run Button</b> on Visual Studio </li>
	</ul>
</li>
</ol>
<br />

<h3>To Run this project: </h3>

<ol>
<li> Running the website
	<ul>
		<li>$ dotnet watch run or Press <b> F5 </b> or <b>Run Button</b> on Visual Studio </li>
	</ul>
</li>
</ol>
<br />


### I will not corver
1. How to create the development environments such as installing software, database, setting up 
development environments. If you haven't set up the development environments, please go to "Requirements"
section and make sure you 

### If you don't know, just followed along with me. 
I will show you step by step with the clear instructions. Please be patient. At the end you will learn 
something. 


## Development Environment Requirements

You will need the following specification. 

### IDE 
Visual Studio 2022 or Newer (preferred)
You may follow along with VisualStudio Code on any cross platforms. 
Visual Studio/Code Link: https://visualstudio.microsoft.com/

### Database 
SQL ServerExpress (Free from Microsoft)
Link: https://www.microsoft.com/en-us/sql-server/sql-server-editions-express

### DotNet Core SDK
DotNet Core SDK Version: 6.0.101
Make sure you download SDK and install it on your machine.
Ref: https://dotnet.microsoft.com/download

### Git 
You will need Git command line. 
