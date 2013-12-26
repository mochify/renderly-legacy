﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using System.Net;
using Microsoft.Win32;

using Renderly.Models;

namespace Renderly.Utils
{
    public class TestCaseGenerator
    {
        private IRenderlyAssetManager _fileManager;

        public TestCaseGenerator(IRenderlyAssetManager fileManager)
        {
            _fileManager = fileManager;
        }

        /// <summary>
        /// This function consumes an iterator over ShellTestCase objects
        /// and yields an iterator over the TestCase objects generated by them.
        /// </summary>
        /// <param name="shells"></param>
        /// <returns></returns>
        public IEnumerable<TestCase> GenerateTestCases(IEnumerable<ShellTestCase> shells)
        {
            foreach (var shell in shells)
            {
                yield return Generate(shell);
            }
        }

        public TestCase Generate(ShellTestCase shell)
        {
            // we have to do something to obtain the reference image
            // and save it to the 'reference' location
            string copiedName = _fileManager.FetchToRandomFilename(shell.ReferenceLocation, shell.ReferenceSavePath);

            var tc = new TestCase
            {
                SourceLocation = shell.ReferenceLocation,
                ReferenceLocation = Path.Combine(shell.ReferenceSavePath, copiedName),
                Type = shell.Type,
                Release = shell.Release,
                Description = shell.Description,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now
            };

            return tc;
        }
    }
}
