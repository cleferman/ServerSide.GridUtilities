<!-- Improved compatibility of back to top link: See: https://github.com/othneildrew/Best-README-Template/pull/73 -->
<a name="readme-top"></a>
<!--
*** Thanks for checking out the Best-README-Template. If you have a suggestion
*** that would make this better, please fork the repo and create a pull request
*** or simply open an issue with the tag "enhancement".
*** Don't forget to give the project a star!
*** Thanks again! Now go create something AMAZING! :D
-->



<!-- PROJECT SHIELDS -->
<!--
*** I'm using markdown "reference style" links for readability.
*** Reference links are enclosed in brackets [ ] instead of parentheses ( ).
*** See the bottom of this document for the declaration of the reference variables
*** for contributors-url, forks-url, etc. This is an optional, concise syntax you may use.
*** https://www.markdownguide.org/basic-syntax/#reference-style-links
-->
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]



<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/cleferman/ServerSide.GridUtilities">
  </a>

<h3 align="center">ServerSide.GridUtilities</h3>

  <p align="center">
    A tiny library which adds typical server side grid actions like filtering, sorting, grouping and column selection.
    <br />
  </p>
</div>

>Note: I have no plans on maintaining this repository at the moment. Feel free to fork it or just simply use it as inspiration. 

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#the-gridrequest-object">The GridRequest object</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>


<!-- ABOUT THE PROJECT -->
## About The Project

ServerSide.GritUtilities is a small library which adds database actions such as filtering, sorting, grouping and column selection to your project. If you work with generic entities or simply have too many properties on your class and you don't want to write queries for each of them this library is for you.

The project aims to showcase the use of C# Expression Trees and how dynamic LINQ queries can be built.
<p align="right">(<a href="#readme-top">back to top</a>)</p>


### Built With

* [.NET 8][.net-url]

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Getting Started

### Installation
ServerSide.GridUtilities is available on [NuGet](https://www.nuget.org/packages/ServerSide.GridUtilities). 
Install it with the following command:
```sh
dotnet add package ServerSide.GridUtilities
```

Use the --version option to specify which version to install.
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage
Add dynamic filtering, sorting, grouping simply by adding the following using statement:

```csharp
using ServerSide.GridUtilities.Extensions;
```

Once the package is added you can use it in your project. Consider the following example which uses Entity Framework Core to connect to a SQL Database:

```csharp
async Task<GridResults<Student>> GetFilteredStudents(GridRequest gridRequest)
{
    //gets all students with selected properties based on the provided filter criteria and ordered by the 
    //sort model.
    var query = dbContext.Students
        .Select(gridRequest.Columns)
        .FilterBy(gridRequest.Filtering)
        .OrderBy(gridRequest.Sorting)
        // Add pagination, skip the first x rows
        .Skip(gridRequest.Pagination.StartRow)
        // Take only the page size
        .Take(gridRequest.Pagination.EndRow - gridRequest.Pagination.StartRow);

    var results = await query.ToListAsync();
    var totalCount = await query.CountAsync();

    //returns a list of paginated students along with a total count so we know if we have more students to 
    //retrieve
    return new GridResults<Student>
    {
        Results = results,
        TotalCount = totalCount
    };
}
```

The neat thing about it is you can replace the Student class with any entity like so:
```csharp
async Task<GridResults<T>> GetFilteredEntity<T>(GridRequest gridRequest) where T : class
{
    //T needs to be a valid entity on your DbContext
    var query = dbContext.Set<T>()
        .Select(gridRequest.Columns)
        .FilterBy(gridRequest.Filtering)
        .OrderBy(gridRequest.Sorting)
        // Add pagination, skip the first x rows
        .Skip(gridRequest.Pagination.StartRow)
        // Take only the page size
        .Take(gridRequest.Pagination.EndRow - gridRequest.Pagination.StartRow);

    var results = await query.ToListAsync();
    var totalCount = await query.CountAsync();

    //returns a list of paginated students along with a total count so we know if we have more students to 
    //retrieve
    return new GridResults<Student>
    {
        Results = results,
        TotalCount = totalCount
    };
}
```
You might be wondering what this GridRequest class is. Below in json format you can see an example of such a request:
```json
{
  //return the first 50 results
  "pagination": {
    "startRow": 0, 
    "endRow": 50
  },
  //Only return data for those 3 columns. The rest are going to be null
  "columns": [
    "Name", "EnrollmentDate", "Age"
  ],
  //sort descending by Age
  "sorting": [
    {
      "colName": "Age",
      "sort": "desc"
    }
  ],
  //Get all students which have 'Student 1' in their name
  "filtering": [
    {
      "conditions": [
        {
          "filterMethod": "contains",
          "values": [
            "Student 1"
          ]
        }
      ],
      "fieldName": "Name",
      "filterType": "text"
    }
  ],
  "grouping": {
    "rowGroupCols": [],
    "groupKeys": []
  }
}
```

If you want to see more examples have a look at the `ServerSide.GridUtilities.Example.API` for some examples. You can also [fork](https://github.com/cleferman/ServerSide.GridUtilities/network/members) this repository and use the project as your playground.

## The GridRequest object
- [Pagination](#pagination)
- [Columns](#columns)
- [Sorting](#sorting)
- [Filtering](#filtering)

### Pagination
When you work with lots of data you always need to retrieve it in chunks or pages. The Pagination has 2 properties:
- StartRow - the value of start row tells us how many rows we skip
- EndRow - the value of end row tells us how pig the page we wish to return is (EndRow - StartRow)
This generates the following SQL Statement

### Columns
If you have lots of columns in your database table it is often preferred to return only a subset of those in certain scenarios. The query can be more performant and the data transferred between the DB and the app is smaller. If you provide an empty array it will select all columns in the table.
>Note: If you provide a column name which does not exist on the type you query it will throw an error.

### Sorting
The sorting model is an array of columns and their sort order. The values for sort are case insensitive:
- asc, ascending, 0
- desc, descending, 1

Any of the above values will be parsed by the `SortType` enum.
>Note: If you provide a column name which does not exist on the type you query it will throw an error.

### Filtering
The following filter types are supported:
- text
- date
- number
> Note: The values for the FilterType are case insensitive

Your filter model can contain as many fields as you have in the model. Between different fields an `AND` operation is applied.
You can have up to two filter criteria on a single field. Between those two criterias you can choose to have an `AND` or an `OR` operation.
```json
// this will retrieve all students whose names contain Student 1 but also equals to Student 2
"filtering": [
   {
     "conditions": [
       {
         "filterMethod": "contains",
         "values": [
           "Student 1"
         ]
       },
       {
         "filterMethod": "equals",
         "values": [
           "Student 2"
         ]
       }
     ],
     "fieldName": "Name",
     "filterType": "text",
     "operator": "OR",
   }
 ],
```
>Note 1: If you don't have multiple conditions on the same field there is no need to specify the Operator property

For filtering on multiple fields consider the following json:
```json
// this will retrieve all students whose name contain Student 1 and whose age is lessThan 20
"filtering": [
  {
    "conditions": [
      {
        "filterMethod": "contains",
        "values": [
          "Student 1"
        ]
      },
    ],
    "fieldName": "Name",
    "filterType": "text",
  },
    {
    "conditions": [
      {
        "filterMethod": "lessThan",
        "values": [
          "20"
        ]
      },
    ],
    "fieldName": "Age",
    "filterType": "number",
  }
],
```
>Note 2: If you provide a field name which does not exist on the type you query it will throw an error.

A condition expects a `filterMethod` and one or multiple `values`. The filter methods:
- text
  - Contains/NotContains - gets all rows where the field contains/does not contain the provided value
  - Equals/NotEqual - gets all rows where the field equals/does not equal with the provided value
  - StartsWith - gets all rows where the field starts with the provided value
  - EndsWith - gets all rows where the field ends with the provided value
  - Blank - gets all rows where the field is empty
  - NotBlank - gets all rows where the field is not empty
  - In - gets all rows where the field is contained in the array of provided values
  - NotIn - gets all rows where the field is not contained in the array of provided values
- number
  - Equals/NotEqual
  - GreaterThan/LessThan - gets all rows where the field is greater than / less than the provided value
  - GreaterThanOrEqual/LessThanOrEqual - gets all rows where the field is greater than or equal / less than or equal the provided value
  - InRange - gets all rows where the field is between the two provided values
  - Blank
  - NotBlank
- date
  - Equals/NotEqual
  - GreaterThan
  - LessThan
  - Blank
  - NotBlank
  
> Note: The values for the FilterMethod are case insensitive

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## Available Extension methods
The following extension methods are added by the library on the IQueryable<T> interface:
```csharp
//A list of column names defined on the T class that you wish to select. If a column is not found on the provided T class an exception will be thrown.
IQueryable<T> Select<T>(this IQueryable<T> queryable, IList<string> columns)
```
```csharp
//A list of sort models which contain the field name and sort direction. The first sort model is going to use an OrderBy LINQ statement, any subsequent models are going to use ThenBy statements.
IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<SortModel> sortModels)
```
```csharp
//Same as above, in case you want to add an OrderBy(t => t.Property) you'll need this method to add dynamic sort
IQueryable<T> ThenOrderBy<T>(this IQueryable<T> source, IEnumerable<SortModel> sortModels)
```

```csharp
//A list of filter models. You can filter after every column present on T. More info here:
IQueryable<T> FilterBy<T>(this IQueryable<T> source, IList<FilterModel> filterModels)
```

```csharp
//TBD
IQueryable<T> GroupBy<T>(this IQueryable<T> source, GroupingModel groupModel)
```

```csharp
//Applies Select, FitlerBy, OrderBy, Skip, Take on the IQueryable<T> in this order
IQueryable<T> ApplyGridRequest<T>(this IQueryable<T> source, GridRequest gridRequest)
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- CONTACT -->
## Contact
Project Link: [https://github.com/cleferman/ServerSide.GridUtilities](https://github.com/cleferman/ServerSide.GridUtilities)

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[contributors-shield]: https://img.shields.io/github/contributors/cleferman/ServerSide.GridUtilities.svg?style=for-the-badge
[contributors-url]: https://github.com/cleferman/ServerSide.GridUtilities/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/cleferman/ServerSide.GridUtilities.svg?style=for-the-badge
[forks-url]: https://github.com/cleferman/ServerSide.GridUtilities/network/members
[stars-shield]: https://img.shields.io/github/stars/cleferman/ServerSide.GridUtilities.svg?style=for-the-badge
[stars-url]: https://github.com/cleferman/ServerSide.GridUtilities/stargazers
[issues-shield]: https://img.shields.io/github/issues/cleferman/ServerSide.GridUtilities.svg?style=for-the-badge
[issues-url]: https://github.com/cleferman/ServerSide.GridUtilities/issues
[license-shield]: https://img.shields.io/github/license/cleferman/ServerSide.GridUtilities.svg?style=for-the-badge
[license-url]: https://github.com/cleferman/ServerSide.GridUtilities/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/cleferman
[.net-url]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

