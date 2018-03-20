using System;

/// <summary>
/// Type of token for piping.
/// </summary>
public enum TokenType
{
    /// <summary>
    /// Answer to a question.
    /// </summary>
    Answer = 0,

    /// <summary>
    /// Value from respondent's profile
    /// </summary>
    Profile,

    /// <summary>
    /// Property of current response.
    /// </summary>
    Response,

    /// <summary>
    /// Property of template associated with current response.
    /// </summary>
    ResponseTemplate,

    /// <summary>
    /// Other
    /// </summary>
    Other
}


