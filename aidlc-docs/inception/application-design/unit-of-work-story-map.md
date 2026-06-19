# Unit of Work — Requirements Map

Since User Stories were skipped (pure refactoring with no new user personas), this document maps **functional requirements** to units instead.

## Requirements-to-Unit Mapping

| Requirement | Unit |
|-------------|------|
| FR-1: Code Deduplication | Unit 1 |
| FR-2: Architecture Separation | Unit 2 |
| FR-3: VB.NET Idiom Replacement | Unit 3 |
| FR-4: config.json Support | Unit 4 |
| FR-7: Template Variables | Unit 5 |
| FR-8: Export with Variable Substitution | Unit 6 |
| FR-5: 40 Macro Set Support | Unit 7 (BLOCKED) |
| FR-6: Scrollbar UI Enhancement | Unit 8 |
| (Undo/Redo from Q5 answer) | Unit 9 |

## NFR Coverage

| NFR | Covered By |
|-----|-----------|
| NFR-1: Behavioral Preservation | All units (testing gate per unit) |
| NFR-2: No Network Access | All units (constraint, not implementation) |
| NFR-3: Target Framework (.NET 4.8) | Baseline (DONE) |
| NFR-4: Testing Strategy | Unit 3 produces regression docs; all units have gates |
| NFR-5: Backward Compatibility | Units 4, 7 |
