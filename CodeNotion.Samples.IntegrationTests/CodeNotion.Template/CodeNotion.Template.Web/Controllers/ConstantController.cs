using System;
using Microsoft.AspNetCore.Mvc;
using CodeNotion.Template.Business.Enums;

namespace CodeNotion.Template.Web.Controllers;

[Route("api/constants")]
public class ConstantController : ControllerBase
{
    [HttpGet(nameof(ErrorType))]
    public ErrorType[] GetErrorTypes() =>
        (ErrorType[]) Enum.GetValues(typeof(ErrorType));
}