So Renderly has these main responsibilities:

* Reads data in from file/database for list of test cases to run

A test case specifies the following:

# URL to hit for a preview
# A location of the reference image (currently a UNC path somewhere)
# The type of test (matching, live preview, dip, blah blah, this is only for processing purposes)

The data model will be:

# Test Case Number (int, PK)
# Test Type (string)
# URL to render (URL/string/blah)
# Reference location (string, two parts - TypeOfLocation:Location)
# Date Added (date)
# Date Modified (date)
# Release Added (string)
# Description (optional, string)

So the flow of the program:

* Start the engine
* Pass in arguments to allow you to specify certain things
 * Test number to 