package com.polytech.contentservice.service.content;

import com.polytech.contentservice.dto.content.ContentDto;
import com.polytech.contentservice.dto.episode.EpisodeDto;
import com.polytech.contentservice.entity.Content;
import com.polytech.contentservice.mapper.ContentMapper;
import com.polytech.contentservice.mapper.EpisodeMapper;
import com.polytech.contentservice.repository.ContentRepository;
import jakarta.transaction.Transactional;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;

import java.util.List;
import java.util.Set;
import java.util.UUID;
import java.util.stream.Collectors;

/**
 * Реализация {@link ContentService}.
 */
@Service
@RequiredArgsConstructor
public class ContentServiceImpl implements ContentService {
    private final ContentRepository contentRepository;
    private final ContentMapper contentMapper;
    private final EpisodeMapper episodeMapper;

    @Override
    public ContentDto createContent(ContentDto contentDto) {
        Content contentToSave = contentMapper.convertToContentEntity(contentDto);
        Content savedContent = contentRepository.save(contentToSave);
        return contentMapper.convertToContentDto(savedContent);
    }

    @Override
    @Transactional
    public void updateContent(UUID id, ContentDto contentDto) {
        Content oldContent = getContentById(id);
        Content updatedContent = contentMapper.patchUpdate(oldContent, contentDto);
        contentRepository.save(updatedContent);
    }

    @Override
    public Set<EpisodeDto> findAllEpisodesForContent(UUID contentId) {
        Content curContent = getContentById(contentId);
        return curContent.getEpisodes()
                .stream()
                .map(episodeMapper::convertToEpisodeDto)
                .collect(Collectors.toSet());
    }

    private Content getContentById(UUID contentId) {
        return contentRepository.findById(contentId)
            .orElseThrow(() -> new IllegalArgumentException("Episode not found"));
    }

    @Override
    public List<ContentDto> findAllContent(Pageable pageable) {
        Page<Content> contents = contentRepository.findAll(pageable);
        return contentMapper.convertToListOfContentDto(contents);
    }

    @Override
    public ContentDto findContentById(UUID id) {
        return contentMapper.convertToContentDto(getContentById(id));
    }
}
