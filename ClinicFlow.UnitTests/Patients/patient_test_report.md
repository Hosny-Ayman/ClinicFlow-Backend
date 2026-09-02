### 1. Selected Feature
**Patients**
*Why it was selected*: After checking the `ClinicFlow.Application\Features` and `ClinicFlow.UnitTests`, it was confirmed that `Patients` is a feature that exists in the application but its `UnitTests\Patients` directory was completely empty. It is also the first on the preferred order list of untested features.

### 2. Production Methods Audited
The `PatientService.cs` has 5 public methods:
1. `CreatePatientAsync(CreatePatientDtoRequest dto)`
2. `GetPatientByIdAsync(int patientId)`
3. `UpdatePatientAsync(UpdatePatientDtoRequest dto)`
4. `GetAllPatientsAsync(PatientSearchDtoRequest request)`
5. `GetPatientInformationForAppointmentAsync(PatientAppointmentSearchDtoRequest search)`

### 3. Test Matrix
Every required scenario from the audit was implemented:
- **`CreatePatientAsync`**
  - Success: Valid request -> Creates patient, adds `ClinicPatient`, saves to DB, returns Patient Id.
- **`GetPatientByIdAsync`**
  - NotFound: `GetPatientByIdAsync` returns null -> Returns `OperationStatus.NotFound`.
  - Success: Patient exists -> Returns mapped DTO.
- **`UpdatePatientAsync`**
  - NotFound: Patient not found (tracking: true) -> Returns `OperationStatus.NotFound` and `SaveChangesAsync` is never called.
  - Success: Patient exists -> Maps properties, saves to DB, returns true.
- **`GetAllPatientsAsync`**
  - Success: Delegates to `_queryService.GetAllPatientsAsync` -> Returns `OperationResult` wrapping response.
- **`GetPatientInformationForAppointmentAsync`**
  - NotFound: `_queryService` returns null -> Returns `OperationStatus.NotFound`.
  - Success: `_queryService` returns response -> Returns mapped response.

### 4. Files Created/Modified
- `[NEW]` `d:\Programming 2026\Clinic Flow\ClinicFlow-BackEnd\ClinicFlow.UnitTests\Common\Builders\PatientBuilder.cs`
- `[NEW]` `d:\Programming 2026\Clinic Flow\ClinicFlow-BackEnd\ClinicFlow.UnitTests\Common\Mocks\PatientMocks.cs`
- `[NEW]` `d:\Programming 2026\Clinic Flow\ClinicFlow-BackEnd\ClinicFlow.UnitTests\Patients\PatientServiceTests.cs`

### 5. Existing Patterns Reused
- Used `CreateService` pattern exactly like `DoctorServiceTests.cs` and `AppointmentServiceTests.cs`.
- Reused `CommonMocks.UnitOfWork()`, `CommonMocks.CurrentUserService()`, `CommonMocks.Logger<PatientService>()`.
- Mapped profiles via `MapperConfiguration(cfg => cfg.AddMaps(typeof(PatientProfile).Assembly))` in the constructor as seen in existing tests.
- Reused xUnit, Moq (`Mock<T>`, `Verify`, `It.IsAny`, `Times`), and `OperationResult` assertions (`Assert.True(result.IsSuccess)`).

### 6. Test Results
**Focused tests (`ClinicFlow.UnitTests.Patients`):**
Passed: 8 / Failed: 0 / Skipped: 0

**Full backend tests:**
Passed: 109 / Failed: 0 / Skipped: 0

### 7. Production Changes
**ZERO production changes.**
No production code was modified to make the tests pass.
