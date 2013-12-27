using System;
using System.Collections.Generic;

namespace Renderly.Models
{
    public interface ITestCaseModel
    {
        /// <summary>
        /// Return an iterator over the test cases in this model.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TestCase> GetTestCases();

        /// <summary>
        /// Return an iterator over the test cases in this model that
        /// match a predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TestCase> GetTestCases(Func<TestCase, bool> predicate);

        /// <summary>
        /// Add a single test case to this model.
        /// </summary>
        /// <param name="t"></param>
        void AddTestCase(TestCase t);

        /// <summary>
        /// Adds multiple test cases to the model.
        /// </summary>
        /// <param name="it"></param>
        void AddTestCases(IEnumerable<TestCase> it);

        /// <summary>
        /// Removes items from this model.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>The number of items deleted from the model.</returns>
        int Delete(Func<TestCase, bool> predicate);

        /// <summary>
        /// Persists the model's data, if necessary.
        /// </summary>
        void Save();
    }
}
