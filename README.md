So Renderly has these main responsibilities:

* Reads data in from file/database for list of test cases to run
* Hits the URLs, grabs the generated render, compares them with a reference image
* Generates a report of the URLs that failed, and for what reason(s) they failed

A test case specifies the following:

1. URL to hit for a preview
2. A location of the reference image (currently a UNC path somewhere)
3. The type of test (matching, live preview, dip, blah blah, this is only for processing purposes)

The data model will be:

* Test Case Number (int, PK)
* Test Type (string)
* URL to render (URL/string/blah)
* Reference location (string, two parts - TypeOfLocation:Location)
* Date Added (date)
* Date Modified (date)
* Release Added (string)
* Description (optional, string)

So the flow of the program:

* Start the engine
* Pass in arguments to allow you to specify certain things
 * Test number(s) to test
 * Types of test(s) to run (like running all things of type 'lp')
* Read in all the tests necessary (numbers, types, everything)
* Process all the things and do all the comparisons
* Generate a report with failures WITH NUSTACHE