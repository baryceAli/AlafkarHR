# Cross-Module Configuration Guide

## Purpose

This guide is the master configuration workbook for the full ERP rollout. It should be used during discovery, build, UAT and go-live readiness to confirm that dependent modules are configured in the correct order.

## Configuration Sequence

| Step | Configuration area | Required decisions | Output |
| --- | --- | --- | --- |
| 1 | Organization foundation | Companies, branches, administrations, departments, business lines and license categories. | Approved organization structure and business-line activation. |
| 2 | Security foundation | User groups, roles, permission policies, branch visibility and administrator accounts. | Approved role model and access matrix. |
| 3 | General settings | System settings, currencies and home page templates. | Frozen system baseline for UAT. |
| 4 | Accounting foundation | Chart of accounts, fiscal periods, tax codes, posting profiles, bank/cash accounts, templates and ZATCA settings. | Approved finance setup and posting design. |
| 5 | Commercial masters | Customers, customer groups, customer pricing profiles, suppliers and supplier groups. | Approved customer and supplier master data. |
| 6 | Product and inventory masters | Products, SKUs, categories, brands, units, variants, packages, pricing, warehouses, inventory records and batches. | Approved catalog, warehouse and opening stock baseline. |
| 7 | HR masters | Employees, positions, teams, shifts, holidays, leave policies, payroll components and payroll structures. | Approved HR and payroll baseline. |
| 8 | Transaction setup | Sales, procurement, stock operations, POS, contracts, projects, tasks, fleet and maintenance operating setup. | Transaction workstreams ready for UAT. |
| 9 | Business-line setup | Catering, Real Estate and StoreFront/POS configurations. | Business-line scenarios ready for UAT. |
| 10 | Reporting and controls | Operational, finance, HR, inventory, sales, procurement and executive reporting. | Approved reconciliation and management reporting pack. |

## Cross-Module Dependency Matrix

| Dependent module | Depends on | Why it matters |
| --- | --- | --- |
| Sales | Customers, pricing, catalog, inventory, accounting. | Quotes, orders, delivery and invoicing need valid customers, prices, items, stock and posting rules. |
| Procurement | Suppliers, catalog, inventory, accounting. | Purchase documents and receipts need valid suppliers, items, warehouses and financial treatment. |
| Inventory | Catalog, warehouses, accounting, security. | Stock movements need products, locations, costing/posting policy and authorized users. |
| POS/StoreFront | Stores, departments, catalog, pricing, inventory, payments, accounting. | POS sales require store context, sellable items, price, stock, payment and reconciliation rules. |
| Payroll | Employees, attendance, leave, accounting. | Salary runs depend on employee contracts, work entries, leave impact and posting policy. |
| Catering | Customers/contracts, catalog, schedules, inventory, accounting. | Catering services require contract terms, meal items, delivery plans and billing/reconciliation rules. |
| Real Estate | Properties, units, parties, contracts, payments, accounting. | Leasing and collections require property/unit data, contract terms and finance rules. |
| Contracts | Customers, suppliers, employees or parties, documents, accounting. | Contract lifecycle needs party data, templates, attachments and financial impact review. |
| Projects and tasks | Organization, employees, customers or internal owners. | Project/task assignment requires users, departments and reporting ownership. |
| Fleet | Employees, branches, maintenance, documents, expenses. | Assignments and fleet costs require responsible users, vehicle documents and service rules. |
| Maintenance | Assets, inventory, employees, fleet. | Work orders depend on maintained assets, spare parts, assignees and escalation rules. |
| Document Management | Security, organization, source records. | Document visibility and retention depend on role access and source process ownership. |

## Configuration Workbook

| Area | Configuration item | Owner | Status | Notes |
| --- | --- | --- | --- | --- |
| Organization | Companies and branches | Project sponsor | Not started | Confirm legal entities and operating branches. |
| Organization | Business lines | System owner | Not started | Validate `catering`, `real-estate` and `store-front`. |
| Security | Roles and permissions | Security administrator | Not started | Map every user to the least-privilege role. |
| Finance | Chart of accounts | Finance manager | Not started | Requires finance sign-off before transaction testing. |
| Finance | Tax and ZATCA | Tax owner | Not started | Requires statutory validation. |
| Sales | Customer groups and pricing | Sales manager | Not started | Validate discount, price and credit policy. |
| Procurement | Supplier groups and scorecard | Procurement manager | Not started | Validate onboarding and evaluation criteria. |
| Warehouse | Warehouses and opening stock | Warehouse manager | Not started | Reconcile inventory before go-live. |
| POS | Stores, cashiers and payment methods | Store operations | Not started | Validate cashier access and daily reconciliation. |
| HR | Employee and payroll masters | HR manager | Not started | Reconcile employee, attendance, leave and payroll data. |
| Catering | Contracts, meals and schedules | Catering manager | Not started | Validate service commitments and delivery proof. |
| Real Estate | Properties, units and leases | Property manager | Not started | Validate lease terms and collection rules. |
| Contracts | Templates and renewals | Legal or contract owner | Not started | Validate approval and renewal policy. |
| Documents | Upload policy and document types | Document controller | Not started | Validate confidentiality and retention rules. |
| Projects/Tasks | Project/task ownership | PMO or operations lead | Not started | Validate project codes, assignment and reporting cadence. |
| Fleet | Vehicles and service rules | Fleet manager | Not started | Validate assignment, document expiry and service thresholds. |
| Maintenance | Assets and work orders | Maintenance manager | Not started | Validate preventive and corrective maintenance workflows. |

## Sign-Off Rule

Every configuration area should have a named owner, completed data template, UAT evidence, open-issue decision, and final approval before production activation.
