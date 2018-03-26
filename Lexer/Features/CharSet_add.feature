Feature: Char set can be extended by further characters
  Scenario: Add single char to empty character set
    Given an empty character set
    When adding a single character "a"
    Then the character set contains "a"
    And the character set has a size of 1
    
  Scenario: Add a range to empty character set
    Given an empty character set
    When adding a character range from "a" to "z"
    Then the character set contains "a"
    And the character set has a size of 26
    And the character set contains "b"
    And the character set contains "c"
    And the character set contains "z"