# Survey
Base project to kick start the Tengella survey test.
## Setup
Feel free to use whatever editors/IDEs.
However we strongly recommend *Visual Studio*.

 1. Download and install *Visual Studio Community Edition* - https://visualstudio.microsoft.com/free-developer-offers/
 2. Make sure to install the following workloads (during installation or after using *Visual Studio Installer*) 
	  - *ASP.NET and web development*
	  - *Data storage and processing*
 3. After loading the solution open the *Package Manager Console* and runt the command `Update-Database`

You should now have a working project that can be debugged by hitting F5.
## Information
The project *Tengella.Survey.Data* contains an example DbContext using EF Core. Confgured to use the database server *(localdb)\MSSQLLocalDB* which is installed along with Visual Studio from the workload *Data storage and processing*.
The  project *Tengella.Survey.WebApp* is the default example for a new ASP.NET MVC project. The Home controller has the DbContext injected for an example on how to use it.

See the test document for more information.

### Links

ASP.NET - https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-7.0
EF Core - https://learn.microsoft.com/en-us/ef/core/

# Plan

## Functionalities

### Survey Creation
- Create and name new surveys
- Specify the type of survey
- Add radio button questions (2-10 options).
- Add open-ended questions for free-text responses.
- Add final free-text information for survey creators
- Preview the survey
- Copy surveys
- Set survey closing dates

### Survey Distribution
- Mock email function to simulate survey distribution
- Create and use email text templates
- Import email addresses from EXCEL/CSV
- Manage recipient lists(add, remove, view)
- Include opt-out links
- Reuse recipient lists for future surveys
- Handle surveys past their closing dates

### Reporting and Analysis
- View previous surveys with details (date, type, recipients, responses)
- Display survey data in forms and charts
- Compare trends over time for specific questions
- Create graphical representations of survey results

## Technologies
- Backend: ASP.NET MVC, C#, EF Core
- Frontend: Razor views, Bootstrap, jQuery
- Database: SQL Server Express LocalDB
- Charts: Chartist or similar library

## Todo

### Database Design and Backend
1. Database Design
- Design tables for surveys, questions, options, responses, users, and distribution lists. [X]
- Create entity models in EF Core. [X]

2. Implement Backend Functions
- Create controllers and services for CRUD operations on surveys and questions. [X]
- Implement business logic for adding, updating, and deleting surveys and questions. [X]
- Add validation rules to ensure data integrity. [X]

### User interface for Survey Creation
1. Create surveys
- Design Razor views for creating and naming new surveys. [X]
- Implement form submissions to create surveys in the database. [X]
- Ensure surveys can be listed, viewed and deleted. [X]

2. Add Questions and Options
- Design Razor views for adding questions and answer options. [X]
- Implement dynamic form fields for adding multiple options. [X]
- Validate input to ensure 2-10 options per question. [X]
- Implement the ability to add open-ended questions. [X]
- Add a preview feature to review the survey before saving. [X]

### Copy Surveys and set Closing Dates
1. Implement Copy Function
- Add a button to copy existing surveys. [X]
- Implement backend logic to duplicate survey data. [X]
- Ensure copied surveys can be edited independently. [X]

2. Add Closing Date Functionality
- Add a date picker for setting survey closing dates. [X]
- Implement backend logic to store and enforce closing dates. [X]
- Display appropriate messages for surveys past their closing dates. [X]

### Survey Distribution
1. Mock Email Function
- Create a mock email service to simulate sending survey links. []
- Design a form to create and manage email templates. []
- Implement backend logic to store and retrieve email templates. []
- Ensure email links redirect to the correct survey pages. []

2. Manage Recipient Lists
- Implement file upload functionality to import email addresses from Excel/CSV. []
- Create forms to add, remove and view recipients manually. []
- Validate email addresses and ensure required fields are populated. []
- Add opt-out links and implement logic to handle opt-outs. []
- Store recipient lists in the database for reuse. []

### Reporting and Analysis
1. Display Survey Data
- Create views to list previous surveys with metadata (date, type, recipients, responses). []
- Design detailed views to display individual survey results. []
- Implement backend logic to aggregate survey data for display. []

2. Graphical Representations and Trends
- Integrate Chartist (or similar library) for graphical representations. []
- Create charts and graphs to visualize survey data. []
- Implement functionality to compare trends over time for specific questions. []
- Add options for different chart types (bar, pie, line). []

### Documentation and Presentation
- Document key functionalities and how to use the system. []
- Prepare a presentation or demo to showcase the system's features. []
- Ensure well-commented and organized code. []

# Potential improvements
- Adding multiple sections within a survey, each with its own set of questions.
- Option to save drafts of surveys not yet ready for distribution.
- Functionality to reorder questions and sections within a survey.
- Option for Reminder emails to non-respondents before survey closing date.
- Functionality to personalize emails.
- Add export functionalities for survey results in formats like CSV/Excel/PDF.
- Add filters to view responses based on certain criterias(date range, respondent demographics?).
- Add ability to mark questions as required or optional.
- Implement WSIWYG text editing for questions and descriptions.
- Implement tooltips