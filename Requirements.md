# ShopBoss Requirements Document:

---

##Summary

This document outlines the requirements for a new Shop Floor Part Tracking System intended to replace the discontinued Production Coach software. The system will manage the millwork manufacturing workflow from CNC cutting through assembly and shipping, supporting a hierarchical data structure and a multi‑station process flow. The goal is a modern, reliable web-based application that addresses current system shortcomings and meets all defined business needs.

## Business Problem Statement

### Current System Limitations

* **Discontinued Support:** The legacy Production Coach system is no longer supported by its vendor.
* **Reliability Issues:** Frequent bugs and crashes in the current system disrupt production.
* **Language Barriers:** The old software’s poor English translations impair usability on the shop floor.
* **Configuration Constraints:** The existing system has inflexible process flows and cannot easily adapt to changing operational needs.
* **Storage Integration Gaps:** Only 6 out of dozens of physical storage racks are configured in the current software.
* **Maintenance Difficulties:** There is no way to customize or update the old system’s behavior, leading to stagnation.

### Business Impact

* Production delays due to system crashes and required restarts.
* Inefficient parts storage and retrieval – over 90 % of rack capacity is unused because the system cannot configure additional racks.
* Operators must perform manual workarounds for basic tasks the software fails to support.
* Increased labor costs and slowdowns caused by system inefficiencies.
* Risk of a **complete production halt** if the unsupported system fails with no recourse.

### Success Criteria for New System

1. **Near‑100 % Uptime.**
2. **Complete Storage Integration** (all racks).
3. **Flexible Process Configuration.**
4. **User‑Friendly Interface** requiring minimal training.
5. **Modern Web Architecture** accessible from any shop‑floor terminal.

---

## Data Structure Requirements

### Hierarchical Data Model

```
Work Order
├── Nest Sheets
├── Products
│   ├── Parts
│   └── Subassemblies
│       └── Parts
├── Hardware
└── Detached Products
```

Ensure parent‑child links and unique IDs are preserved.

### Data Integrity Requirements

* Preserve unique IDs from Microvellum.
* Maintain parent‑child links throughout workflow.
* Track part status independently yet linked to parents.
* Track hardware and detached products separately.

---

## Functional Requirements (FR)

### FR‑001: Work Order Import and Management

*High priority.* Import `.sdf` files, parse hierarchy, preserve IDs, handle errors, archive files.

**Acceptance Criteria:**

* AC‑001.1 SQL CE file detection within 30 s.
* AC‑001.2 Hierarchical data import with correct links.
* AC‑001.3 Identifier preservation.
* AC‑001.4 Duplicate work‑order rejection.
* AC‑001.5 Graceful error handling for corrupt files.

### FR‑002: Multi‑Station Process Tracking

Stations: Cut, Sort, Assembly, Shipping. Enforce sequence, real‑time updates, batch cut scanning.

### FR‑003: Dynamic Storage Management

Unlimited racks with custom dimensions; slot = rack/row/col; random assignment balancing; visual rack display.

### FR‑004: Hierarchical Part Organization

Group parts by product; handle single‑part, carcass, multi‑part subassemblies; compute product completion.

### FR‑005: Assembly Coordination

Track component completion, show subassembly locations, notify readiness, generate pick list, one‑scan completion.

### FR‑006: Shipping Verification

Generate shipping checklists, include hardware/detached items, enforce assembled‑only rule, produce documentation.

---

## Non‑Functional Requirements (NFR)

### NFR‑001 Performance

* ≤ 2 s scan response; concurrent use; handle 500 products/5 000 parts orders.

### NFR‑002 Reliability

* 99.9 % uptime; auto‑reconnect; crash‑safe restart.

### NFR‑003 Usability

* Responsive web UI; learnable in 30 min; large touch‑friendly controls; ≤ 3 clicks for common ops.

### NFR‑004 Compatibility

* Runs in modern browsers on Win 10/11; supports Code 39 barcodes; .NET 8 backend.

### NFR‑005 Scalability

* Add stations, racks, higher volume without major refactor.

---

## User Stories

### Admin

* **US‑001 Admin: Import Work Orders** — \*As an \****Admin***, I need to import a work order from Microvellum in SQL CE (`.sdf`) format so that the system captures all relevant data (work‑order header, products, parts, cut sheets, nests, etc.) in its own database.
* **US‑002 Admin: Manage Work Orders** — \*As an \****Admin***, I need to view, delete, and modify work orders in the database so that production data remains accurate and up to date.
* **US‑003 Admin: Manage Sorting Racks** — \*As an \****Admin***, I need to add, delete, and define compartments (rows/columns) in each sorting rack so that digital storage matches the physical warehouse.
* **US‑004 Admin: Override Storage Locations** — \*As an \****Admin***, I need to manually override the contents of any storage location so that I can correct mistakes or handle exceptions.

### CNC Operator

* **US‑005 CNC: Batch Cut Scanning** — \*As a \****CNC operator***, I need to scan the barcode on a nest sheet and have the software automatically mark all related parts as **Cut** so that I avoid dozens of individual scans.

### Sorting Operator

* **US‑006 Sort: Slot Assignment** — \*As a \****Sorting operator***, I need the software to tell me where to store a part once I scan its barcode so that parts are organized efficiently.
* **US‑007 Sort: Rack Visualization** — \*As a \****Sorting operator***, I need a visual view of the current sorting rack showing which slots are filled/empty and highlighting where to place the scanned part.
* **US‑008 Sort: Product Grouping** — \*As a \****Sorting operator***, I need the software to recognize parts that belong to the same product and instruct me to store them together in preparation for assembly.
* **US‑009 Sort: Special Rack for Doors/Drawer Fronts** — \*As a \****Sorting operator***, I need the software to recognize when a part is a door or drawer front and direct me to the special rack reserved for those items.
* **US‑010 Sort: Status Update** — \*As a \****Sorting operator***, when I sort a part, the software must update that part’s status to **Sorted**.

### Assembly Operator

* **US‑011 Assembly: Carcass Readiness & Completion** — \*As an \****Assembly operator***, I need the software to alert me when all carcass parts of a product are sorted, and after assembly, allow me to scan any part to mark the entire product (and its parts) as **Assembled**, then direct me to the stored doors and drawer fronts for fitting.

### Shipping Operator

* **US‑012 Shipping: Work‑Order Verification** — \*As a \****Shipping operator***, I need to pull up a work order and scan each product, hardware item, and detached product as it is loaded so that the software tallies everything and prevents omissions. The system updates the status for each part and each product I handle to "Shipped".

*(Additional user stories for Production Managers, QA, and future enhancements remain in the Requirements for further elaboration.)*

---

## Integration & System Testing Criteria

INT‑001 Barcode integration, INT‑002 multi‑station coordination, INT‑003 end‑to‑end import‑to‑ship.

## Acceptance Criteria Overview

Feature complete, integration tests pass, performance met, UAT done, docs updated.

---