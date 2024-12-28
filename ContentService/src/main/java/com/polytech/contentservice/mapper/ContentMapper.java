package com.polytech.contentservice.mapper;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.entity.Content;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;

import java.time.LocalDateTime;
import java.util.Collections;
import java.util.List;
import org.springframework.stereotype.Component;

/**
 * Маппер для Content.
 */
@Component
@RequiredArgsConstructor
public class ContentMapper {
    private final CastMemberMapper castMemberMapper;

    public ContentDto convertToContentDto(Content content) {
        return ContentDto.builder()
                .id(content.getId())
                .title(content.getTitle())
                .ageRestriction(content.getAgeRestriction())
                .genre(content.getGenre())
                .category(content.getCategory())
                .quality(content.getQuality())
                .description(content.getDescription())
                .publisher(content.getPublisher())
                .thumbnail(content.getThumbnail())
                .castMembers(content.getCastMembers() == null ? Collections.emptySet() : castMemberMapper.convertToSetOfCastMemberDto(content.getCastMembers()))
                .build();
    }

    public Content convertToContentEntity(ContentDto contentDto) {
        return Content.builder()
                .title(contentDto.title())
                .ageRestriction(contentDto.ageRestriction())
                .genre(contentDto.genre())
                .category(contentDto.category())
                .creationDate(LocalDateTime.now())
                .updatedDate(LocalDateTime.now())
                .quality(contentDto.quality())
                .description(contentDto.description())
                .publisher(contentDto.publisher())
                .thumbnail(contentDto.thumbnail())
                .build();
    }

    public List<ContentDto> convertToListOfContentDto(Page<Content> contents) {
        return contents.stream()
                .map(this::convertToContentDto)
                .toList();
    }

    public Content patchUpdate(Content oldEpisodeDetail, ContentDto contentDto) {
        return Content.builder()
                .id(oldEpisodeDetail.getId())
                .title(contentDto.title() != null ? contentDto.title() : oldEpisodeDetail.getTitle())
                .ageRestriction(contentDto.ageRestriction() != null ? contentDto.ageRestriction() : oldEpisodeDetail.getAgeRestriction())
                .genre(contentDto.genre() != null ? contentDto.genre() : oldEpisodeDetail.getGenre())
                .category(contentDto.category() != null ? contentDto.category() : oldEpisodeDetail.getCategory())
                .quality(contentDto.quality() != null ? contentDto.quality() : oldEpisodeDetail.getQuality())
                .description(contentDto.description() != null ? contentDto.description() : oldEpisodeDetail.getDescription())
                .publisher(contentDto.publisher() != null ? contentDto.publisher() : oldEpisodeDetail.getPublisher())
                .thumbnail(contentDto.thumbnail() != null ? contentDto.thumbnail() : oldEpisodeDetail.getThumbnail())
                .updatedDate(LocalDateTime.now())
                .creationDate(oldEpisodeDetail.getCreationDate())
                .build();
    }
}
