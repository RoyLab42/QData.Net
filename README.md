# QDataLite.Net

QDataLite.Net is a DotNet library that makes the data query more flexible.

For the origin of QDataLite.Net, please refer to this [blog post](https://blog.ruolinz.com/2019/10/the-design-of-an-elastic-rest-api/).

## Introduction

It used to be a nightmare when implementing MVC controllers for different kinds of data accesses. Take the `PersonModel` as an example, you may implement different kinds of services to satisfy different user requirements such as:

```csharp
[HttpGet("/genderis/{gender}")]
public async Task<IActionResult> GetPersonsByGender([FromRoute] int gender){}

[HttpGet("/youngerthan/{age}")]
public async Task<IActionResult> GetPersonsYoungerThan([FromRoute] int age){}

[HttpGet("/olderthan/{age}")]
public async Task<IActionResult> GetPersonsOlderThan([FromRoute] int age){}
```

Luckily, with the help of the QDataLite.Net library, you can make the server-side MVC controller extremely simpler. It is possible to achieve requirements in only one method, and there are more benefits.

## Get Started

### Data Query

Take the `PersonModel` as an example:

```csharp
public class PersonModel
{
    public Guid Id { get; set; }
    public int Age { get; set; }
    public int Gender { get; set; }
    public string Name { get; set; }
}
```

To provide the data access service, one function is good enough:

```csharp
[HttpGet]
[Authorize]
public IActionResult Query([FromQuery] string selector, [FromQuery] string filter,
    [FromQuery] string orderBy)
{
    var persons = dbContext.Set<PersonModel>()
                           .QueryDynamic(selector, filter, orderBy);
    return Ok(persons);
}
```

This service enables the client-side more flexibility on any data query they want.

- `selector` is a list of property names separated by commas (,)
- `filter` is a query expression in the form of [Polish Notation](https://en.wikipedia.org/wiki/Polish_notation)
- `orderBy` is a list of property names with prefix + or -, separated by semi-colons (;)

Here are some examples for quick start-up with this powerful library.

> For simplicity, the parameters below were not URL-encoded, please remember to encode the values in real web requests

**Example 1**

get name and age of persons, whose age is equal or older than 18, order by age

```
/api/person?selector=Name,Age&filter=Age>=18&orderBy=+Age
```

**Example 2**: get persons whose name is `Roy`

```
/api/person?filter=Name=Roy
```

**Example 3**: get males whose age is between [25, 50), order by age in decending order

```
/api/person?filter=&(&(Age>=25)(Age<50))(Gender=1)&orderBy=-Age
```

## Report issues, Request new features

Feel free to report or request at: https://github.com/RoyLab42/QData.Net/issues

## Related blog posts

- [API design enhancement for filtering in Asp.Net Core 3.0](https://blog.ruolinz.com/2019/10/api-design-enhancement-for-filtering-in-asp-net-core-3-0/)
- [Tailor the API without code change in Asp.Net Core 3.0](https://blog.ruolinz.com/2019/11/tailor-the-api-without-code-change-in-asp-net-core-3-0/)
- [Sort dataset of REST API flexibly in Asp.Net Core 3.0](https://blog.ruolinz.com/2019/11/sort-dataset-of-rest-api-flexibly-in-asp-net-core-3-0/)
