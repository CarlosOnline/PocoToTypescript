cls
@echo.
C:\temp\PocoToTypescript\bin\Debug\PocoToTypescriptGenerator.exe ^
 Test.cs ^
 Test2.cs ^
 -o Tests\Test1.d.ts

@echo.
 C:\temp\PocoToTypescript\bin\Debug\PocoToTypescriptGenerator.exe ^
 Test.cs ^
 Test2.cs ^
 -o Tests\Test2.d.ts ^
 --ExcludedAttributes=JsonIgnore,NotMapped,TestClass,TestProp
