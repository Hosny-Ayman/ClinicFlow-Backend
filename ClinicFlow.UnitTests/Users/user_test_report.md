### 1. Selected Feature
**Users**
*Why it was selected*: As requested, this was the direct next target after `Patients`. The `UnitTests\Users` directory had no tests.

### 2. Production Methods Audited
The `UserService.cs` has 8 public methods:
1. `GetCurrentUserAsync()`
2. `CreateReceptionistAsync(CreateAndEditUserDtoRequest userDto)`
3. `AddUserInsideProjectOnlyAsync(CreateAndEditUserDtoRequest userDto)`
4. `GetUserInformationByIdAsync(int userId)`
5. `UpdateUserAsync(UpdateUserInformationDtoRequest userDto)`
6. `UpdateUserInsideProjectOnlyAsync(User user, UpdateUserInformationDtoRequest dto)`
7. `ToggleUserStatusAsync(int userId)`
8. `GetAllReceptionistsformationsAsync(ReceptionistsSearchDtoRequest request)`

### 3. Test Matrix
Every required scenario from the audit was implemented (16 tests total):
- **`GetCurrentUserAsync`**: 
  - Unauthorized (IsAuthenticated == false).
  - NotFound (User is null).
  - Success (Returns `CurrentUserDto`).
- **`CreateReceptionistAsync`**: 
  - Conflict (User email/phone already exists).
  - Success (Creates user, hashes password, assigns Receptionist role, saves to DB).
- **`GetUserInformationByIdAsync`**: 
  - Forbidden (Cannot manage user).
  - NotFound (User not found).
  - Success (Returns mapped DTO).
- **`UpdateUserAsync`**: 
  - Forbidden (Cannot manage user).
  - NotFound (User not found).
  - Success without Password (Password remains unchanged).
  - Success with Password (New password is hashed and saved).
- **`ToggleUserStatusAsync`**: 
  - BadRequest (User does not exist).
  - Failure (Toggle operation fails in repository).
  - Success (Status is toggled successfully).
- **`GetAllReceptionistsformationsAsync`**: 
  - Success (Returns PagedResponse).

### 4. Files Created/Modified
- `[NEW]` `d:\Programming 2026\Clinic Flow\ClinicFlow-BackEnd\ClinicFlow.UnitTests\Common\Builders\UserBuilder.cs`
- `[NEW]` `d:\Programming 2026\Clinic Flow\ClinicFlow-BackEnd\ClinicFlow.UnitTests\Users\UserServiceTests.cs`

*(Note: Did not create `UserMocks.cs` because `IUserRepository` was already mocked in `AuthenticationMocks` and `IUserRoleRepository` in `DoctorMocks`, following instructions to heavily reuse existing files).*

### 5. Existing Patterns Reused
- Used `CreateService` pattern.
- Reused `UserQueryMocks.UserQueryService()`, `AuthenticationMocks.UserRepository()`, `DoctorMocks.UserRoleRepository()`, `DoctorMocks.CheckService()`, `CommonMocks.UnitOfWork()`.
- Added mapping profile for `UserProfile` natively without extending external dependencies.
- Relied on xUnit, Moq, and matched `OperationStatus` assertions to match all returned `OperationResult` wrapper types, including correctly capturing `BCrypt` static hashing where applicable.

### 6. Test Results
**Focused tests (`ClinicFlow.UnitTests.Users`):**
Passed: 16 / Failed: 0 / Skipped: 0

**Full backend tests:**
Passed: 125 / Failed: 0 / Skipped: 0

### 7. Production Changes
**ZERO production changes.**
No production code was touched or modified.
