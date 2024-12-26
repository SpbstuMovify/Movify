package com.polytech.contentservice.common;

import lombok.Getter;
import lombok.RequiredArgsConstructor;

/**
 * Возрастные ограничения
 */
@Getter
@RequiredArgsConstructor
public enum AgeRestriction {
    SIX_PLUS("6+"),
    TWELVE_PLUS("12+"),
    SIXTEEN_PLUS("16+"),
    EIGHTEEN_PLUS("18+");

    private final String restriction;
}
