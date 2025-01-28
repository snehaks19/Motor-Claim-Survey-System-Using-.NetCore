# **Motor Survey System - ASP.NET Core Web Application**

This repository contains the source code for the **Motor Survey System**, a robust web-based application developed in **.NET Core**. It facilitates the management of motor insurance policies, claims, and surveys, leveraging **Controller Actions** and **RESTful APIs** for efficient backend operations.

---

## **Key Features**

### **Policy Management**
- Create and manage motor insurance policies.
- Support for key policy details like policy number, premium amounts, and policyholder information.

### **Claim Management**
- File and manage claims with details such as vehicle information, claim status, and loss descriptions.
- Integration with **Oracle Database** for secure storage and retrieval of claim records.
- Attach surveys to claims for additional details.

### **Survey Management**
- Manage surveys linked to claims, tracking details such as:
  - **Item Codes**
  - **Item Types**
  - **Unit Prices**
  - **Quantities**
  - Calculated amounts in **Foreign Currency (FC)** and **Local Currency (LC)**.
- Automated calculations for **LC** and **FC** amounts based on user input.
- Display historical survey data in a responsive grid with pagination and proper formatting.
- Validation and dynamic form updates based on user actions (e.g., dropdown selections, text input).

---

## **Technologies Used**
- **.NET Core MVC** for server-side logic and API development.
- **RESTful APIs** for backend operations and data exchange.
- **Oracle Database** for data storage, with stored procedures and triggers for business logic.
- **Bootstrap** for a clean, responsive user interface.
- **JavaScript** and **AJAX** for dynamic updates and user interactions.
- **SweetAlert** for intuitive notifications and alerts.
---


This project demonstrates the use of modern **.NET Core** practices, RESTful APIs, and Oracle Database integration to build scalable and secure web applications.
