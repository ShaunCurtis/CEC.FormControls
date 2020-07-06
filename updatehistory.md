# Update History

## 1.0.1

###### CEC.FormControls:

1. Adding a Cascading Action Parameter to the FormRecordControl Class called OnRecordChange. This duplicates ChangedFromRecord.
1. Change the source of the return value for ChangedFromRecord to directly EditContext.IsModified().
1. Change to the FormControlTextArea to output the value as "Content" i.e. between the opening and closing tags, rather than as a "value" of the html control.
1. Added AsMarkup parameter to FormControlPlainText and set default to output value as MarkUpString.

###### Sample Project:

1. Adding and wiring the cascading callback.
1. Cleanup of the RecordChange callbacks/events.

## 1.0.2

###### CEC.FormControls:

1. Refactoring of CurrentValue code.

###### Sample Project:

1. Updated code to use latest CEC.Routing options.
1. Migrated editor code to EditComponentBase module.
3. General code cleanup and typo fixes.