# **Motor Survey System - ASP.NET Core Web Application**

This repository contains the source code for the **Motor Survey System**, a web-based application developed in **.NET Core**. The system facilitates efficient management of motor insurance policies, claims, and surveys, with separate workflows for employees and surveyors. The application includes dashboards with graphical overviews of policy, claim, and survey statuses.

---

## **Project Workflow**

### **Users**
- **Employee**: Responsible for policy registration, claim submission, and surveyor assignment.
- **Surveyor**: Conducts surveys, tracks vehicle part details, and submits estimated loss amounts.

### **Process Flow**
1. **Policy Registration**:  
   - An **employee** registers a new policy for a client.  
   - After approval, the policy becomes active and ready for claims.  

2. **Claim Submission**:  
   - The **employee** submits a claim if a loss occurs within the policy period.  
   - Claim details include loss description, dates, and vehicle information.  

3. **Claim Intimation**:  
   - Once the claim is submitted, the **employee** intimates the claim to a surveyor.  

4. **Survey Assignment**:  
   - The insurance company assigns a **surveyor** to evaluate the loss.  

5. **Surveyor Action**:  
   - The **surveyor** records details of damaged vehicle parts, including item codes, unit prices, and quantities.  
   - Estimated amounts (both **FC** and **LC**) are calculated and submitted.  

6. **Claim Approval**:  
   - The **employee** reviews the survey and either approves or rejects the claim:  
     - If **approved**, the user receives a detailed printout of all claim, survey, and policy details.  
     - If **rejected**, appropriate actions are communicated to the surveyor or client.  

7. **Dashboard Overview**:  
   - A dashboard displays real-time charts and statistics for policy, claim, and survey statuses.

---

## **Key Features**

### **Policy Management**
- Create and manage policies with details like policy number, client information, and premium amounts.
- Policies are validated to ensure accurate entry before activation.

### **Claim Management**
- Submit claims for losses occurring within the policy period.
- Assign claims to surveyors and track claim statuses.
- Claims can be approved or rejected based on survey results.

### **Survey Management**
- Surveyors track and manage damaged vehicle parts:
  - Record item codes, types, unit prices, and quantities.
  - Calculate estimated amounts in **Foreign Currency (FC)** and **Local Currency (LC)**.
- Submit final survey reports for review and approval.

### **Reports**
- Generate **Crystal Reports** with all policy, claim, and survey details, including the final claim amount.
- Reports are printable and serve as a complete record for clients and internal use.

### **Dashboard**
- A graphical overview of key metrics:
  - Active policies, claims, and surveys.
  - Status summaries (approved, rejected, pending).
  - Easy-to-interpret charts for decision-making.

---

## **Master Tables**

### **Codes Master**
- Stores dropdown values used across the application, ensuring consistency and easy updates.

### **Error Code Master**
- Centralized storage of alert messages displayed throughout the system.

### **User Master**
- Maintains user details and roles:
  - **Employee**: Manages policies, claims, and survey assignments.
  - **Surveyor**: Conducts surveys and submits estimated reports.

---

## **Technologies Used**
- **.NET Core MVC** for server-side logic and Controller Actions.
- **RESTful APIs** for backend operations and data exchange.
- **Crystal Reports** for generating detailed reports.
- **Oracle Database** with stored procedures and triggers for business logic.
- **Bootstrap** for responsive and user-friendly design.
- **JavaScript** and **AJAX** for dynamic form updates.
- **SweetAlert** for interactive alerts and notifications.
- **Charts** for graphical data representation on the dashboard.

---

This project showcases a complete motor insurance workflow, combining the power of **.NET Core**, **APIs**, **Oracle Database**, and **Crystal Reports** to deliver an efficient, scalable, and user-friendly web application.
