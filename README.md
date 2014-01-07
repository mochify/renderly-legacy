# Renderly

Renderly is a .NET library and library for automated comparison of images. It is intended for use in environments where you want to periodically check generated images against a reference image to see if any changes have caused the images to render differently or incorrectly.

## Source Code Layout

The code is broken up into a few projects, described below.

### Renderly

This is the core library which provides the basic interfaces for building an application.

### RenderlyApp

This is a reference application built on top of Renderly that takes a CSV file with test cases, runs image comparisons against the test cases, and generates a report of results.

### RenderlyTests

This project has all the unit tests for the Renderly library.

## Documentation

Read the wiki!

## Build

You will need Visual Studio 2012 or Visual Studio 2013 to build. I assume the code should build fine in Visual Studio 2010, but it has not been tested with that version.

I have not provided a build script yet, so right now you should open `Renderly` and `RenderlyApp` in a solution and rebuild.

## Running RenderlyApp

This applies to if you're using the included RenderlyApp. Running the `RenderlyApp.exe` without any arguments should produce the list of supported commands:

    > .\RenderlyApp.exe -h

    Available commands are:

        batchadd    - Add test cases as a batch
        deletetest  - Delete test cases (and their reference images) from a model
        run         - Run a rendering job
        updateref   - Update reference images for provides test cases.

        help <name> - For help with one of the above commands

### `batchadd`

The `batchadd` command allows you to create a new test case file, or append test cases to an existing one. Note that if you append, it does not perform an in-place modification of the input file.

The command takes the following arguments:

     > .\RenderlyApp.exe help batchadd

    'batchadd' - Add test cases as a batch

    Expected usage: RenderlyApp.exe batchadd <options>
    <options> available:
    -f, --file=VALUE           CSV file with the test cases to generate.
    -o, --out=VALUE            CSV file to output with test cases.
    -a, --append=VALUE         Test Case file to append to

The file you pass to the `-f` argument is a CSV file that should start with the following header:

    Type,Reference Location,Reference Save Directory,Release,Description

* The `Type` column is the type of test this is.
* The `Reference Location` column is a URI to the image where the image should be copied from. This image becomes the test case's reference image, and the location is where the test will obtain the image from in the future.
* `Reference Save Directory` is the directory where the reference image will be saved.
* `Release` is what software/version release this test was originally created for, or what version it should apply to. This is mostly a reference field in case you are curious about what version/release you were testing an image in.
* `Description` is a short comment about the test case. Only used for your reference.


### `deletetest`

The `deletetest` command allows you to delete a test case from a file.

The command takes the following arguments:

    > .\RenderlyApp.exe help deletetest

    'deletetest' - Delete test cases (and their reference images) from a model

    Expected usage: RenderlyApp.exe deletetest <options>
    <options> available:
      -f, --file=VALUE           Model file to delete test cases from.
      -d, --dates=VALUE          Comma-separated list of dates to delete test
                                   cases for. MM-DD-YYYY format.
      -r, --releases=VALUE       Comma-separated list of releases to delete test
                                   cases for
      -t, --testids=VALUE        Comma-separated list of test IDs to delete
      -o, --outfile=VALUE        File to save test cases to. If no file is
                                   specified, the input file is overwritten.

Options should be rather self-explanatory. You can specify multiple different parameters to erase from a test case file. Any reference images will also be deleted from their saved location, if possible (may not be possible due to permissions or other accessibility issues).

### `updateref`

The `updateref` command lets you update test cases, making the application redownload reference images. This may be useful in case your images or rendering changes, and you want to update the reference image a test case uses.

The command takes the following arguments:

    > .\RenderlyApp.exe help updateref

    'updateref' - Update reference images for provides test cases.

    Expected usage: RenderlyApp.exe updateref <options>
    <options> available:
      -f, --file=VALUE           Model file to update test cases in.
      -t, --testids=VALUE        Comma-separated list of test IDs to redownload
                                   reference images for.

### `run`

The `run` command lets you run a rendering and comparison test, using an input CSV file. A report will be generated detailing the results of the run.

The command takes the following arguments:

    > .\RenderlyApp.exe help run

    'run' - Run a rendering job

    Expected usage: RenderlyApp.exe run <options>
    <options> available:
      -f, --file=VALUE           The model to get test cases from.
      -n, --name=VALUE           The name of the report to generate
      -o, --outdir=VALUE         The directory to generate the report in
      -m, --templatedir=VALUE    The directory to get templates for report
                                   generation
          --threshold=VALUE      Threshold value to configure how aggressive
                                   image comparison is (0-100). 100 is exact match.
                                   Defaults to 100.
          --showall              Show all results in report (including successes-
                                   ). By default, only failures are shown.
          --copyref              Copy reference images locally to report
                                   directory. Default false.
      -t, --testids=VALUE        Comma-separated list of test IDs to run
      -r, --releases=VALUE       Comma-separated list of releases to run
      -d, --dates=VALUE          Comma-separated list of dates to run. MM-DD-YYYY
                                   format. Runs for Date Added.
      -y, --types=VALUE          Comma-separated list of types to run.

The CSV file you pass to the `-f` option should start with the header:

    Test Id,Type,Source Location,Reference Location,Date Added,Date Modified,Release,Description

* `Test Id` is the number of this test. It should be a unique identifying number.
* `Type` is the type of test this is.
* `Source Location` is the URI to obtain the image for comparison.
* `Reference Location` is the URI to the base image that is used for comparisons.
* `Date Added` is the date this test was added to the model.
* `Date Modified` is the date this test was last changed/updated.
* `Release` is the software release/version this test was originally created against.
* `Description` is a short comment about this test case.

A valid CSV file can be obtained by using the `batchadd` command.