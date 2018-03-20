using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkbox.Analytics.Filters
{
    /// <summary>
    /// Collection of filters associated with an analysis.  The collection class supports grouping of filters for display, evaluation,
    /// or (depending on the contained filter types) generating a SQL where clause for extracting answer data.
    /// </summary>
    [Serializable]
    public class AnalysisFilterCollection : FilterDataCollection
    {
        /// <summary>
        /// Get the name of the parent type for the <see cref="FilterDataCollection"/>
        /// </summary>
        /// <remarks>This value has a static value of "AnalysisTemplate".</remarks>
        public override string ParentType
        {
            get { return "AnalysisTemplate"; }
        }

        /// <summary>
        /// Get a string representation of any <see cref="IQueryFilter"/> objects contained in this collection.
        /// </summary>
        /// <remarks>This value is used internally when selecting responses to load into the <see cref="AnalysisData"/> object associated with an
        /// running <see cref="Analysis"/>.</remarks>
        /*public virtual string FilterString
        {
            get
            {
                List<Filter> filters = GetFilters(string.Empty);

                //Build the filter clauses

                return filters.OfType<IQueryFilter>().Aggregate(string.Empty, (current, f) => f is ResponseFilter ? 
                    AppendFilterString(current, f.FilterString) :
                    AppendFilterInClause(current, f.FilterString, f.UseNotIn));
            }
        }*/

        /// <summary>
        /// Build filter string
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, StringBuilder> BuildFilterStrings()
        {
            Dictionary<string, StringBuilder> res = new Dictionary<string, StringBuilder>();
            List<Filter> filters = GetFilters(string.Empty);
            foreach (var f in filters)
            {
                var key = f.FilterParameter;
                if (!res.ContainsKey(key))
                    res.Add(key, new StringBuilder());
                if (f is ResponseFilter)
                { 
                    AppendFilterString(res[key], ((ResponseFilter)f).FilterString);
                }
                else
                {
                    if (f is IQueryFilter)
                    {
                        AppendFilterInClause(res[key], ((IQueryFilter)f).FilterString, ((IQueryFilter)f).UseNotIn);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Append a string to the specified filter string with an appropriate joiner and return the result.
        /// </summary>
        /// <param name="existingFilterString">String to append filter string to.</param>
        /// <param name="toAppend">String to append to the existing filter string.</param>
        /// <returns>Updated filter string with SQL joiner (AND)</returns>
        /// <remarks>Analysis filters only support AND operations, but this may change in the future.</remarks>
        public string AppendFilterString(string existingFilterString, string toAppend)
        {
            if (existingFilterString != null && existingFilterString.Trim() != string.Empty)
            {
                existingFilterString += " AND ";
            }

            return existingFilterString + toAppend;
        }

        /// <summary>
        /// Append a SQL IN clause to the specified filter string.
        /// </summary>
        /// <param name="existingFilterString">Existing filter string to append clause to.</param>
        /// <param name="clause">Filter clause to append.</param>
        /// <param name="negated">Specify whether the clause should be append as a NOT IN clause.</param>
        /// <returns>Updated filter SQL with appended clause.,</returns>
        public string AppendFilterInClause(string existingFilterString, string clause, bool negated)
        {
            if (clause != null && clause.Trim() == string.Empty)
            {
                return existingFilterString;
            }
            
            var sb = new StringBuilder();

            sb.Append(existingFilterString);

            if (existingFilterString != null && existingFilterString.Trim() != string.Empty)
            {
                sb.Append(" AND ");
            }

            sb.Append("( ckbx_Response.ResponseID");

            sb.Append(negated ? " NOT IN " : " IN ");

            sb.Append("(Select ResponseID FROM ckbx_ResponseAnswers WHERE ");
            sb.Append(clause);
            sb.Append("))");

            return sb.ToString();
        }


        /// <summary>
        /// Append a string to the specified filter string with an appropriate joiner and return the result.
        /// </summary>
        /// <param name="existingFilterString">String to append filter string to.</param>
        /// <param name="toAppend">String to append to the existing filter string.</param>
        /// <returns>Updated filter string with SQL joiner (AND)</returns>
        /// <remarks>Analysis filters only support AND operations, but this may change in the future.</remarks>
        public void AppendFilterString(StringBuilder existingFilterString, string toAppend)
        {
            if (existingFilterString.Length > 0)
            {
                existingFilterString.Append(" AND ");
            }

            existingFilterString.Append(toAppend);
        }

        /// <summary>
        /// Append a SQL IN clause to the specified filter string.
        /// </summary>
        /// <param name="existingFilterString">Existing filter string to append clause to.</param>
        /// <param name="clause">Filter clause to append.</param>
        /// <param name="negated">Specify whether the clause should be append as a NOT IN clause.</param>
        /// <returns>Updated filter SQL with appended clause.,</returns>
        public void AppendFilterInClause(StringBuilder sb, string clause, bool negated)
        {
            if (clause != null && clause.Trim() == string.Empty)
            {
                return;
            }

            if (sb.Length > 0)
            {
                sb.Append(" AND ");
            }

            sb.Append("( ckbx_Response.ResponseID");

            sb.Append(negated ? " NOT IN " : " IN ");

            sb.AppendFormat("(Select ResponseID FROM ckbx_ResponseAnswers WHERE {0}))", clause);
        }


        ///<summary>
        /// Returns date of the last modified filter
        ///</summary>
        public DateTime? GetLastModifiedDate()
        {
            DateTime? lastModifield = null;
            foreach (var filterData in GetFilterDataObjects())
            {
                filterData.Load();
                if (!lastModifield.HasValue || lastModifield.Value < filterData.LastModified)
                {
                    lastModifield = filterData.LastModified;
                }
            }

            return lastModifield;
        }
    }
}