# Business Line Implementation Guide

## Purpose

This guide explains how to implement the confirmed Alafkar business lines alongside the core ERP modules. The confirmed business-line keys are `catering`, `real-estate`, and `store-front`.

## Business-Line Summary

| Business line | Confirmed key | Primary users | Core scope | Required validation |
| --- | --- | --- | --- | --- |
| Catering | `catering` | Catering operations, delivery coordinators, contract managers, finance reviewers. | Contracts, meals, locations, schedules, deliveries, assignments and reports. | Contract terms, meal plans, delivery proof, billing triggers and exception handling. |
| Real Estate | `real-estate` | Property managers, leasing officers, collection users, finance reviewers. | Properties, units, owner leases, tenant leases, collections, utilities, expenses and reports. | Lease templates, rent schedules, owner settlement, utility recovery, legal and finance treatment. |
| StoreFront/POS | `store-front` | Store managers, cashiers, POS supervisors, inventory and finance users. | POS, stores, departments, organization and store management. | Cashier access, POS sessions, payment methods, stock impact, refund policy and daily reconciliation. |

## Catering Implementation

### Setup Checklist

| Setup item | Owner | Result |
| --- | --- | --- |
| Catering contracts | Contract manager | Active customer or internal service contracts are loaded and approved. |
| Meal catalog | Catering operations | Meal items, package names and operational descriptions are validated. |
| Locations | Operations lead | Delivery and service locations are complete and linked to responsible teams. |
| Schedules | Scheduling owner | Contract schedules, meal times and delivery windows are ready for UAT. |
| Assignments | Operations lead | Users or teams are assigned to operational responsibilities. |
| Reports | Business owner | Daily, schedule and delivery reports are reconciled against expected operations. |

### UAT Scenarios

| Scenario | Expected result |
| --- | --- |
| Create or review a catering contract | Contract details are visible and usable by authorized users. |
| Maintain meal data | Meal information can be found and used in schedules. |
| Create a schedule | Schedule appears for the correct location and delivery period. |
| Track delivery | Delivery status and exceptions can be reviewed. |
| Review reports | Business owner can confirm service volume and exception reporting. |

## Real Estate Implementation

### Setup Checklist

| Setup item | Owner | Result |
| --- | --- | --- |
| Properties | Property manager | Property records are loaded with correct ownership and location details. |
| Units | Leasing officer | Units are linked to properties and have clear availability status. |
| Owner leases | Legal/finance owner | Owner lease data is approved and ready for accounting review. |
| Tenant leases | Leasing officer | Tenant lease terms, dates and rent schedules are validated. |
| Collections | Finance owner | Collection process, receipts and overdue tracking are validated. |
| Utilities and expenses | Property accountant | Utility recovery and expense allocation are approved. |
| Reports | Business owner | Occupancy, collection, utility and expense reports are approved. |

### UAT Scenarios

| Scenario | Expected result |
| --- | --- |
| Add property and units | Property hierarchy is complete and searchable. |
| Create tenant lease | Lease data is captured with correct tenant, unit, period and financial details. |
| Record collection | Collection is visible for finance review and reporting. |
| Record utility or expense | Cost is linked to the correct property or unit. |
| Review real estate reports | Business owner can validate occupancy, collections and expenses. |

## StoreFront/POS Implementation

### Setup Checklist

| Setup item | Owner | Result |
| --- | --- | --- |
| StoreFront organization | Store operations | Stores are mapped to the right company, branch and departments. |
| Stores | Store manager | Store records are ready and assigned to responsible users. |
| Departments | Store operations | Departments match the operating model and reporting needs. |
| POS users | Security administrator | Cashiers and supervisors have correct permissions. |
| POS inventory | Warehouse owner | Store stock, products, SKUs and pricing are validated. |
| Payment setup | Finance owner | Payment methods and reconciliation process are approved. |

### UAT Scenarios

| Scenario | Expected result |
| --- | --- |
| Open POS page | Authorized user can access the POS workspace. |
| Select store context | Store and department context is clear to the cashier. |
| Sell product | Product, price and stock behavior match approved policy. |
| Process payment | Payment method is captured for reconciliation. |
| Review end-of-day process | Store manager and finance reviewer can reconcile expected totals. |

## Cross-Business-Line Controls

| Control | Applies to | Required evidence |
| --- | --- | --- |
| Business-line activation | Catering, Real Estate, StoreFront/POS | Business line appears in organization setup and relevant menu visibility is confirmed. |
| Role access review | All business lines | Security owner signs off user access by role and branch. |
| Master data ownership | All business lines | Each master data table has a named data owner. |
| Finance impact review | All business lines | Accounting owner validates billing, payment, collection, posting and reporting implications. |
| Support handover | All business lines | Support team receives known issues, escalation path and owner list. |
