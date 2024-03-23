using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Common.Response;

/// <summary>
/// Contains fields and methods to handle the error and response, this class needs to inherited by all command and query handlers
/// </summary>
public class ResponseWrapper<T>
{
    /// <summary>
    /// Returns the overall api repsonse status as success along with the data and success messages if any
    /// </summary>
    /// <param name="response"></param>
    /// <param name="responseMessages"></param>
    /// <returns></returns>
    public ResponseVm<T> Return200WithData(dynamic response, params ResponseMessage[] responseMessages)
    {
        T obj1 = (T)Activator.CreateInstance(typeof(T));

        Type type = obj1.GetType();

        var concretObj = Convert.ChangeType(response, type);

        return new ResponseVm<T>() { Data = concretObj, ResponseMessages = responseMessages.Select(x => new ResponseMessage() { Description = x.Description, Status = x.Status, Name = x.Name }).ToList(), StatusCode = System.Net.HttpStatusCode.OK };
    }

    /// <summary>
    /// Returns the overall api repsonse status as success along with the list of success messages
    /// </summary>
    /// <param name="responseMessages"></param>
    /// <returns></returns>
    public ResponseVm<T> Return200WithMessages(params ResponseMessage[] responseMessages)
    {
        return new ResponseVm<T>() { ResponseMessages = responseMessages.Select(x => new ResponseMessage() { Description = x.Description, Status = x.Status, Name = x.Name }).ToList(), StatusCode = System.Net.HttpStatusCode.OK };
    }

    /// <summary>
    /// Returns the overall api repsonse status as fail along with the list of error messages
    /// </summary>
    /// <param name="responseMessages"></param>
    /// <returns></returns>
    public ResponseVm<T> Return400(params ResponseMessage[] responseMessages)
    {
        return new ResponseVm<T>() { ResponseMessages = responseMessages.Select(x => new ResponseMessage() { Description = x.Description, Status = x.Status, Name = x.Name }).ToList(), StatusCode = System.Net.HttpStatusCode.BadRequest };
    }

    //Method to handle Errors by using a list of messages
    public ResponseVm<T> Return400List(List<ResponseMessage> responseMessages)
    {
        return new ResponseVm<T>()
        {
            ResponseMessages = responseMessages.Select(x => new ResponseMessage()
            {
                Description = x.Description,
                Status = x.Status,
                Name = x.Name
            }).ToList(),
            StatusCode = System.Net.HttpStatusCode.BadRequest
        };
    }
    public ResponseVm<T> Return400WithData(dynamic response, params ResponseMessage[] responseMessages)
    {
        T obj1 = (T)Activator.CreateInstance(typeof(T));

        Type type = obj1.GetType();

        var concretObj = Convert.ChangeType(response, type);

        return new ResponseVm<T>() { Data = concretObj, ResponseMessages = responseMessages.Select(x => new ResponseMessage() { Description = x.Description, Status = x.Status, Name = x.Name }).ToList(), StatusCode = System.Net.HttpStatusCode.BadRequest };
    }

}
/// <summary>
/// 
/// </summary>
public enum ResponseStatus
{
    Success,
    Error,
    Warning
}

/// <summary>
/// 
/// </summary>
public class ResponseMessage
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public ResponseStatus Status { get; set; }
}