# Requirements
Your submission must be your original work. No more than a combined total of 30% of the submission and no more than a 10% match to any one individual source can be directly quoted or closely paraphrased from sources, even if cited correctly. An originality report is provided when you submit your task that can be used as a guide.

You must use the rubric to direct the creation of your submission because it provides detailed criteria that will be used to evaluate your work. Each requirement below may be evaluated by more than one rubric aspect. The rubric aspect titles may contain hyperlinks to relevant portions of the course.

You are not allowed to use frameworks or external libraries. The database does not contain data, so it needs to be populated. You must use “test” as the user name and password to login.

### A.
Create a log-in form that can determine the user’s location and translate log-in and error control messages (e.g., “The username and password did not match.”) into the user’s language and in one additional language.  
- [X] Login Form
- [X] Language Checking
- [X] Error Messages for improper logins ( # Partially Coded but turned off for testing)

### B.
Provide the ability to add, update, and delete customer records in the database, including name, address, and phone number.  
- [X] Add Customers  
- [X] Edit Customers  
- [X] Delete Customers  

### C.
Provide the ability to add, update, and delete appointments, capturing the type of appointment and a link to the specific customer record in the database.  
- [X] Add Appointment  
- [X] Edit Appointment  
- [X] Delete Appointment  

### D.
Provide the ability to view the calendar by month and by week.
- [X] Calender By Month
- [X] Calender By week

### E.
Provide the ability to automatically adjust appointment times based on user time zones and daylight saving time.
- [X] Auto adjust Appt times by Timezone / DST

### F.
Write exception controls to prevent each of the following. You may use the same mechanism of exception control more than once, but you must incorporate at least two different customized mechanisms of exception control.

- [X] scheduling an appointment outside business hours (ValidationRules)
- [X] scheduling overlapping appointments (Implemented custom OverlappingAppointmentException)
- [X] entering nonexistent or invalid customer data (ValidationRules)
- [x] entering an incorrect username and password (Exceptions tied to resx resources)

### G.
Write two or more lambda expressions to make your program more efficient, justifying the use of each lambda expression with an in-line comment.
- [X] 1st Lambda (ReminderViewModel.cs, Line 22)
- [X] 2nd Lambda (ReportViewModel.cs, Line 35)

### H.
Write code to provide reminders and alerts 15 minutes in advance of an appointment, based on the user’s log-in.
- [X] Appt Reminders

### I.
Provide the ability to generate each  of the following reports using the collection classes:
- [X] number of appointment types by month
- [X] the schedule for each  consultant
- [X] one additional report of your choice 

### J.
Provide the ability to track user activity by recording timestamps for user log-ins in a .txt file, using the collection classes. Each new record should be appended to the log file, if the file already exists.
- [X] Record logins in .txt file (appending)

### K.
Demonstrate professional communication in the content and presentation of your submission.
- [] Everything is awesome