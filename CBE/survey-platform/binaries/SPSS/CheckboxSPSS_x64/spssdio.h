/*
**++
**  NAME
**    spssdio.h - Interface header for the SPSS Data File I/O API.
**
**  DESCRIPTION
**    Code which calls any SPSS Data File I/O API functions must include this
**    header
**--
**  COPYRIGHT
**    (c) Copyright 2002 by SPSS Inc.
**
**  Modifications:
**
**  04 May 2004 - Bartley - longer strings
**  29 Dec 2004 - Bartley - longer value labels
**  10 Feb 2005 - mholubow - reduced max value label limit from 255 to 120
*/

#ifndef SPSSDIO_H
#define SPSSDIO_H

#include "spssdiocodes.h"       /* codes are in a separate file */

#if defined(__cplusplus)
    extern "C" {
#endif

/* We need __stdcall for Windows but not for Unix
*/
#if (defined(_MSC_VER) && !defined(_WIN32)) || \
      (defined(__BORLANDC__) && !defined(__WIN32__))
    /* Looks like 16 bit Windows */
#   error The 16 bit Windows environment is no longer supported
#elif defined(_WIN32) || defined(__WIN32__)
    /* 32 bit Windows - don't do anything */
#else
    /* outside Windows */
#   define __stdcall
#endif

/*  For describing one multiple response set */
typedef struct spssMultRespDef_T
{
    char szMrSetName[SPSS_MAX_VARNAME+1];  /* Null-terminated MR set name */
    char szMrSetLabel[SPSS_MAX_VARLABEL+1];  /* Null-terminated set label */
    int qIsDichotomy;              /* Whether a multiple dichotomy set */
    int qIsNumeric;                /* Whether the counted value is numeric */
    int qUseCategoryLabels;        /* Whether to use var category labels */
    int qUseFirstVarLabel;         /* Whether using var label as set label */
    int Reserved[14];              /* Reserved for expansion */
    long nCountedValue;            /* Counted value if numeric */
    char* pszCountedValue;         /* Null-terminated counted value if string */
    char** ppszVarNames;           /* Vector of null-terminated var names */
    int nVariables;                /* Number of constituent variables */
} spssMultRespDef;


/****************************************** API FUNCTIONS IN ALPHABETIC ORDER
*/

int __stdcall spssAddFileAttribute(
    const int hFile,
    const char* attribName,
    const int attribSub,
    const char* attribText);

int __stdcall spssAddMultRespDefC(
    const int hFile,
    const char* mrSetName,
    const char* mrSetLabel,
    const int isDichotomy,
    const char* countedValue,
    const char* * varNames,
    const int numVars);

int __stdcall spssAddMultRespDefExt(
    const int hFile,
    const spssMultRespDef* pSet);

int __stdcall spssAddMultRespDefN(
    const int hFile,
    const char* mrSetName,
    const char* mrSetLabel,
    const int isDichotomy,
    const long countedValue,
    const char* * varNames,
    const int numVars);

int __stdcall spssAddVarAttribute(
    const int hFile,
    const char* varName,
    const char* attribName,
    const int attribSub,
    const char* attribText);

int __stdcall spssCloseAppend(
    const int hFile);

int __stdcall spssCloseRead(
    const int hFile);

int __stdcall spssCloseWrite(
    const int hFile);

int __stdcall spssCommitCaseRecord(
    const int hFile);

int __stdcall spssCommitHeader(
    const int hFile);

int __stdcall spssConvertDate(
    const int day,
    const int month,
    const int year,
    double* spssDate);

int __stdcall spssConvertSPSSDate(
    int* day,
    int* month,
    int* year,
    const double spssDate);

int __stdcall spssConvertSPSSTime(
    long* day,
    int* hourh,
    int* minute,
    double* second,
    const double spssDate);

int __stdcall spssConvertTime(
    const long day,
    const int hour,
    const int minute,
    const double second,
    double* spssTime);

int __stdcall spssCopyDocuments(
    const int fromHandle,
    const int toHandle);

int __stdcall spssFreeAttributes(
    char** attribNames,
    char** attribText,
    const int nAttributes);

int __stdcall spssFreeDateVariables(
    long* dateInfo);

int __stdcall spssFreeMultRespDefs(
    char* mrespDefs);

int __stdcall spssFreeMultRespDefStruct(
    spssMultRespDef* pSet);

int __stdcall spssFreeVarCValueLabels(
    char* * values,
    char* * labels,
    const int numLabels);

int __stdcall spssFreeVariableSets(
    char* varSets);

int __stdcall spssFreeVarNames(
    char* * varNames,
    int* varTypes,
    const int numVars);

int __stdcall spssFreeVarNValueLabels(
    double* values,
    char* * labels,
    const int numLabels);

int __stdcall spssGetCaseSize(
    const int hFile,
    long* caseSize);

int __stdcall spssGetCaseWeightVar(
    const int hFile,
    char* varName);

int __stdcall spssGetCompression(
    const int hFile,
    int* compSwitch);

int __stdcall spssGetDateVariables(
    const int hFile,
    int* numofElements,
    long* * dateInfo);

int __stdcall spssGetDEWFirst(
    const int hFile,
    void* pData,
    const long maxData,
    long* nData);

int __stdcall spssGetDEWGUID(
    const int hFile,
    char* asciiGUID);

int __stdcall spssGetDEWInfo(
    const int hFile,
    long* pLength,
    long* pHashTotal);

int __stdcall spssGetDEWNext(
    const int hFile,
    void* pData,
    const long maxData,
    long* nData);

int __stdcall spssGetEstimatedNofCases(
    const int hFile,
    long* caseCount);

int __stdcall spssGetFileAttributes(
    const int hFile,
    char*** attribNames,
    char*** attribText,
    int* nAttributes);

int __stdcall spssGetFileCodePage(
    const int hFile,
    int* nCodePage);

int __stdcall spssGetFileEncoding(
    const int hFile,
    char* pszEncoding);

int __stdcall spssGetIdString(
    const int hFile,
    char* id);

int __stdcall spssGetInterfaceEncoding();

int __stdcall spssGetMultRespCount(
    const int hFile,
    int* nSets);

int __stdcall spssGetMultRespDefByIndex(
    const int hFile,
    const int iSet,
    spssMultRespDef** ppSet);

int __stdcall spssGetMultRespDefs(
    const int hFile,
    char** mrespDefs);

int __stdcall spssGetMultRespDefsEx(
    const int hFile,
    char** mrespDefs);

int __stdcall spssGetNumberofCases(
    const int hFile,
    long* caseCount);

int __stdcall spssGetNumberofVariables(
    const int hFile,
    int* numVars);

int __stdcall spssGetReleaseInfo(
    const int hFile,
    int relInfo[]);

int __stdcall spssGetSystemString(
    const int hFile,
    char* sysName);

int __stdcall spssGetTextInfo(
    const int hFile,
    char* textInfo);

int __stdcall spssGetTimeStamp(
    const int hFile,
    char* fileDate,
    char* fileTime);

int __stdcall spssGetValueChar(
    const int hFile,
    const double varHandle,
    char* value,
    const int valueSize);

int __stdcall spssGetValueNumeric(
    const int hFile,
    const double varHandle,
    double* value);

int __stdcall spssGetVarAlignment(
    const int hFile,
    const char* varName,
    int* alignment);

int __stdcall spssGetVarAttributes(
    const int hFile,
    const char* varName,
    char*** attribNames,
    char*** attribText,
    int* nAttributes);

int __stdcall spssGetVarCMissingValues(
    const int hFile,
    const char* varName,
    int * missingFormat,
    char* missingVal1,
    char* missingVal2,
    char* missingVal3);

int __stdcall spssGetVarColumnWidth(
    const int hFile,
    const char* varName,
    int* columnWidth);

int __stdcall spssGetVarCompatName(
    const int hFile,
    const char* longName,
    char* shortName);

int __stdcall spssGetVarCValueLabel(
    const int hFile,
    const char* varName,
    const char* value,
    char* label);

int __stdcall spssGetVarCValueLabelLong(
    const int hFile,
    const char* varName,
    const char* value,
    char* labelBuff,
    const int lenBuff,
    int* lenLabel);

int __stdcall spssGetVarCValueLabels(
    const int hFile,
    const char* varName,
    char* * * values,
    char* * * labels,
    int* numofLabels);

int __stdcall spssGetVarHandle(
    const int hFile,
    const char* varName,
    double* varHandle);

int __stdcall spssGetVariableSets(
    const int hFile,
    char* * varSets);

int __stdcall spssGetVarInfo(
    const int hFile,
    const int iVar,
    char* varName,
    int* varType);

int __stdcall spssGetVarLabel(
    const int hFile,
    const char* varName,
    char* varLabel);

int __stdcall spssGetVarLabelLong(
    const int hFile,
    const char* varName,
    char* labelBuff,
    const int lenBuff,
    int* lenLabel);

int __stdcall spssGetVarMeasureLevel(
    const int hFile,
    const char* varName,
    int* measureLevel);

int __stdcall spssGetVarNames(
    const int hFile,
    int* numVars,
    char* * * varNames,
    int* * varTypes);

int __stdcall spssGetVarNMissingValues(
    const int hFile,
    const char* varName,
    int* missingFormat,
    double* missingVal1,
    double* missingVal2,
    double* missingVal3);

int __stdcall spssGetVarNValueLabel(
    const int hFile,
    const char* varName,
    const double value,
    char* label);

int __stdcall spssGetVarNValueLabelLong(
    const int hFile,
    const char* varName,
    const double value,
    char* labelBuff,
    const int lenBuff,
    int* lenLabel);

int __stdcall spssGetVarNValueLabels(
    const int hFile,
    const char* varName,
    double* * values,
    char* * * labels,
    int* numofLabels);

int __stdcall spssGetVarPrintFormat(
    const int hFile,
    const char* varName,
    int* printType,
    int* printDec,
    int* printWidth);

int __stdcall spssGetVarWriteFormat(
    const int hFile,
    const char* varName,
    int* writeType,
    int* writeDec,
    int* writeWidth);

void __stdcall spssHostSysmisVal(
    double* missVal);

int __stdcall spssIsCompatibleEncoding(
    const int hFile,
    int* bCompatible);

void __stdcall spssLowHighVal(
    double* lowest,
    double* highest);

int __stdcall spssOpenAppend(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenAppendU8(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenRead(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenReadU8(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenWrite(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenWriteU8(
    const char* fileName,
    int* hFile);

int __stdcall spssOpenWriteCopy(
    const char* fileName,
    const char* dictFileName,
    int* hFile);

int __stdcall spssOpenWriteCopyU8(
    const char* fileName,
    const char* dictFileName,
    int* hFile);

int __stdcall spssQueryType7(
    const int hFile,
    const int subType,
    int* bFound);

int __stdcall spssReadCaseRecord(
    const int hFile);

int __stdcall spssSeekNextCase(
    const int hFile,
    const long caseNumber);

int __stdcall spssSetCaseWeightVar(
    const int hFile,
    const char* varName);

int __stdcall spssSetCompression(
    const int hFile,
    const int compSwitch);

int __stdcall spssSetDateVariables(
    const int hFile,
    const int numofElements,
    const long* dateInfo);

int __stdcall spssSetDEWFirst(
    const int hFile,
    const void* pData,
    const long nBytes);

int __stdcall spssSetDEWGUID(
    const int hFile,
    const char* asciiGUID);

int __stdcall spssSetDEWNext(
    const int hFile,
    const void* pData,
    const long nBytes);

int __stdcall spssSetFileAttributes(
    const int hFile,
    const char** attribNames,
    const char** attribText,
    const int nAttributes);

int __stdcall spssSetIdString(
    const int hFile,
    const char* id);

int __stdcall spssSetInterfaceEncoding(
    const int iEncoding);

const char* __stdcall spssSetLocale(
    const int iCategory,
    const char* pszLocale);

int __stdcall spssSetMultRespDefs(
    const int hFile,
    const char* mrespDefs);

int __stdcall spssSetTempDir(
    const char* dirName);

int __stdcall spssSetTextInfo(
    const int hFile,
    const char* textInfo);

int __stdcall spssSetValueChar(
    const int hFile,
    const double varHandle,
    const char* value);

int __stdcall spssSetValueNumeric(
    const int hFile,
    const double varHandle,
    const double value);

int __stdcall spssSetVarAlignment(
    const int hFile,
    const char* varName,
    const int alignment);

int __stdcall spssSetVarAttributes(
    const int hFile,
    const char* varName,
    const char** attribNames,
    const char** attribText,
    const int nAttributes);

int __stdcall spssSetVarCMissingValues(
    const int hFile,
    const char* varName,
    const int missingFormat,
    const char* missingVal1,
    const char* missingVal2,
    const char* missingVal3);

int __stdcall spssSetVarColumnWidth(
    const int hFile,
    const char* varName,
    const int columnWidth);

int __stdcall spssSetVarCValueLabel(
    const int hFile,
    const char* varName,
    const char* value,
    const char* label);

int __stdcall spssSetVarCValueLabels(
    const int hFile,
    const char* * varNames,
    const int numofVars,
    const char* * values,
    const char* * labels,
    const int numofLabels);

int __stdcall spssSetVariableSets(
    const int hFile,
    const char* varSets);

int __stdcall spssSetVarLabel(
    const int hFile,
    const char* varName,
    const char* varLabel);

int __stdcall spssSetVarMeasureLevel(
    const int hFile,
    const char* varName,
    const int measureLevel);

int __stdcall spssSetVarName(
    const int hFile,
    const char* varName,
    const int varType);

int __stdcall spssSetVarNMissingValues(
    const int hFile,
    const char* varName,
    const int missingFormat,
    const double missingVal1,
    const double missingVal2,
    const double missingVal3);

int __stdcall spssSetVarNValueLabel(
    const int hFile,
    const char* varName,
    const double value,
    const char* label);

int __stdcall spssSetVarNValueLabels(
    const int hFile,
    const char* * varNames,
    const int numofVars,
    const double* values,
    const char* * labels,
    const int numofLabels);

int __stdcall spssSetVarPrintFormat(
    const int hFile,
    const char* varName,
    const int printType,
    const int printDec,
    const int printWidth);

int __stdcall spssSetVarWriteFormat(
    const int hFile,
    const char* varName,
    const int writeType,
    const int writeDec,
    const int writeWidth);

double __stdcall spssSysmisVal(
    void);

int __stdcall spssValidateVarname(
    const char* varName);

int __stdcall spssWholeCaseIn(
    const int hFile,
    void* caseRec);

int __stdcall spssWholeCaseOut(
    const int hFile,
    const void* caseRec);

#ifdef __cplusplus
    }
#endif

#endif
