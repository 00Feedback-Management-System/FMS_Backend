Feedback Management Systems

Aim - The System is designed to streamline the Process of collecting, managing and analyzing students feedbacks for courses, modules and faculty members.

Technologies Used Frontend: React.js (Vite, Axios, Material UI, Bootstrap) Backend: ASP.NET Core Web API Database: Microsoft SQL Server ORM: Entity Framework Core Authentication: JWT, ASP.NET Identity Deployment: Azure (Backend), Vercel (Frontend) Version Control: Git & GitHub Testing: Swagger, Postman

Login

We Implement Role- based Authentication and Authorization for this Project, there are three main Roles such as Admin, Student, Staff | Trainer (Theory ( Lab)
Admin

This is main role because Admin can create Feedback Types with adding different Questions from Feedback Types menu on student dashboard also admin can schedule Feedbacks for student for courses, modules, staffs from Schedule feedback menu

⁃ ﻿﻿Admin can view course wise Report with ratings such as Course wise. module wise Aug Rating ⁃ ﻿﻿faculty feedback summary - In this list only Admin can see Submitted response according to student answer’s. ⁃ ﻿﻿Per faculty feedback Summary - In this menu he can Generate Report as in Bar chart, where he can see per faculty report out of rating 5.

Admin Credentials Email - nilesh@gmail.com Password - Nilesh@1234

Student ⁃ In side student login he can see the List of scheduled feedbacks for student according to his course and group. ⁃ ﻿﻿Student can Open&fill feedback and fill/Answer the Questions which will be scheduled by Admin for Particular Course, Module, and faculty. ⁃ ﻿﻿After succesfully submission the record, It will goes in feedback History list, Where he can view his submitted response ⁃ ﻿﻿I applied server side pagination for both list. to Reduce Api response time. Here I set 5 second par page so that so every time a this api retums 5 records per page

Student credentials Email - suhas@gmail.com Password - Suhas@1234

Also, If student is new, he can register herself from registered here link on login page.

Staff ⁃ In side staff login can see the list of overall average rating according to modules

Staff credentials Email - ketan@gmail.com Password - ketan44@

Also Admin can add staffs from add staff menu from admin login

We hosted this Web application on cloud infrastructure for optimal scalability and performance

Live Application Link - https://feedback-front-end-sage.vercel.app/
