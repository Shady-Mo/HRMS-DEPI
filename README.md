# HRMS Project (Human Resource Management System)

Welcome to the **HRMS Project**! This repository is a robust, web-based application designed to streamline and enhance Human Resource management processes. Built using the **MVC (Model-View-Controller)** architecture, the HRMS Project offers a clean separation of concerns, scalability, and maintainability, making it suitable for organizations of all sizes.

---

## 🌟 Key Highlights

- **MVC Architecture**: A well-structured design pattern ensuring a clear separation of concerns between Models, Views, and Controllers for better scalability and manageability.
- **Comprehensive HR Management**: Covers a wide range of HR functionalities, including employee records, payroll, leave management, and more.
- **User-Friendly Interface**: Intuitive and responsive design for seamless user experience across devices.
- **Secure Authentication and Authorization**: Role-based access to the system ensures data privacy and security.
- **Extensible and Modular**: Built with flexibility in mind, allowing easy customization and feature additions.

---

## 📋 Table of Contents

- [About the Project](#about-the-project)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Architecture Overview](#architecture-overview)
- [Authentication and Authorization](#authentication-and-authorization)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)

---

## 📖 About the Project

The **HRMS Project** is designed to simplify and automate HR operations, enabling organizations to focus on their core activities. With its robust feature set and modular architecture, HRMS ensures a seamless experience for managing HR workflows.

---

## ✨ Features

### Core Functionalities:
- **Employee Management**:
  - Add, update, and manage employee profiles.
  - Track employee attendance and performance.

- **Department Management**:
  - Organize employees into departments.
  - Assign roles and responsibilities.

- **Payroll Management**:
  - Automate salary processing and generate payslips.

- **Leave Management**:
  - Process leave requests and track leave balances.

- **Project and Task Management**:
  - Manage projects and assign tasks to employees.
  - Monitor project progress and deadlines.

### Advanced Features:
- **Reports and Analytics**:
  - Generate insightful HR reports.
  - Visualize data through charts and graphs.

- **Authentication and Authorization**:
  - Secure login and role-based access control.

- **Notifications**:
  - Email and in-app notifications for important updates.

---

## 💻 Technologies Used

This project utilizes modern web technologies for a robust and efficient solution:

- **Frontend**:
  - **HTML**: For structuring web pages.
  - **CSS**: For responsive and visually appealing designs.
  - **JavaScript**: For dynamic and interactive UI components.

- **Backend**:
  - **C#**: Implements business logic and controllers (ASP.NET Core MVC).

- **Database**:
  - SQL Server (or alternative relational DB, configurable).

---

## 🏛 Architecture Overview

This project follows the **MVC (Model-View-Controller)** design pattern:

- **Models**:
  - Represent the data and business logic of the system.
  - Examples include `Employee`, `Department`, `Project`, and `Leave`.

- **Views**:
  - Define the user interface and presentation logic.
  - Built using Razor syntax in `.cshtml` files for dynamic rendering.

- **Controllers**:
  - Handle user input, interact with models, and render appropriate views.
  - Example controllers include `EmployeeController`, `ProjectController`, and `AuthController`.

The project also implements a **Repository Pattern** for organizing data access logic, ensuring modularity and maintainability.

---

## 🔐 Authentication and Authorization

The HRMS Project incorporates robust **authentication** and **authorization** mechanisms to ensure secure access control and user management. These features are implemented using **ASP.NET Identity** with role-based access control.

### Authentication
- **Login Process**:
  - Users authenticate using their email and password via the `AuthController`. 
  - The `SignInManager` validates credentials and manages user sessions.
  - Invalid login attempts (e.g., wrong email or password) are handled with clear error messages.

- **Registration**:
  - New users are registered through the `EmployeeController`'s `SaveAdd` action method.
  - This method validates input data, checks for existing users with the same email or username, and assigns roles (e.g., `Employee`, `Manager`).

  Example Code:
  ```csharp
  public IActionResult SaveAdd(EmployeeDTO employeeDTO) {
      // Validate input and check for duplicates
      // Create new Employee instance
      // Assign role and save to database
  }
  ```

- **Password Management**:
  - Passwords are securely hashed using ASP.NET Identity's built-in functionality.

### Authorization
- **Role-Based Access Control (RBAC)**:
  - The project uses roles such as **Admin**, **Manager**, and **Employee** to control access to specific features.
  - Role validation is performed using the `UserManager` and `SignInManager`.

- **Policy Enforcement**:
  - Controllers and actions are decorated with `[Authorize]` attributes to enforce access restrictions.
  - Example: Only employees and managers can access features like profile and leave management.

### Additional Features
- **Session Management**:
  - User sessions are managed securely, and users can log out at any time.

- **Redirect to Dashboards**:
  - Upon successful login, users are redirected to role-specific dashboards (e.g., Admin, Manager, Employee).

---

## 🛠 Installation

### Prerequisites
- Install [Visual Studio](https://visualstudio.microsoft.com/) with .NET Core/ASP.NET support.
- Install [SQL Server](https://www.microsoft.com/en-us/sql-server) or your preferred relational database.
- Install [Node.js](https://nodejs.org/) (if frontend dependencies are managed via npm).

### Steps
1. Clone the repository:
   ```bash
   git clone https://github.com/basmallasaid/HRMS_Project.git
   ```
2. Navigate to the project directory:
   ```bash
   cd HRMS_Project
   ```
3. Set up the database:
   - Create a new database in SQL Server.
   - Run the SQL scripts provided in the `db` directory to create the schema and populate data.

4. Configure the connection string:
   - Update the connection string in `appsettings.json` with your database credentials.

5. Run the application:
   - Open the `.sln` file in Visual Studio.
   - Restore NuGet packages and build the solution.
   - Start the application using `Ctrl+F5`.

---

## 🚀 Usage

1. **Access the Application**:
   - Open your browser and navigate to `http://localhost:[port]`.

2. **Explore Features**:
   - Log in using the default admin credentials or create a new user account.
   - Navigate through the dashboard to access various modules like Employee Management, Payroll, and Leave Management.

3. **Customize Settings**:
   - Update organizational settings to tailor the system to your needs.

---

## 🤝 Contributing

We welcome contributions to enhance the HRMS Project. To get started:

1. Fork the repository and create your feature branch:
   ```bash
   git checkout -b feature/YourFeatureName
   ```
2. Make your changes and commit them:
   ```bash
   git commit -m "Add your feature"
   ```
3. Push to the branch:
   ```bash
   git push origin feature/YourFeatureName
   ```
4. Open a pull request.

---

## 🔗 Contact

- **Author**: [Shady-Mo](https://github.com/Shady-Mo)
- **Repository**: [HRMS Project](https://github.com/basmallasaid/HRMS_Project)
- **Issues**: [Report an Issue](https://github.com/basmallasaid/HRMS_Project/issues)