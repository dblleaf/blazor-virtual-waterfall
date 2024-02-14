import { DotNet } from '@microsoft/dotnet-js-interop';

let scrollContainer: HTMLElement = null;

const findClosestScrollContainer = (element: HTMLElement | null): HTMLElement | null => {
    if (!element || element === document.body || element === document.documentElement) {
        return null;
    }

    const style = getComputedStyle(element);

    if (style.overflowY !== 'visible') {
        return element;
    }

    return findClosestScrollContainer(element.parentElement);
}

const dispatcherObserversByDotNetIdPropname = Symbol();

const getObserversMapEntry = (dotNetHelper: DotNet.DotNetObject): { observersByDotNetObjectId: { [id: number]: any }, id: number } => {
    const dotNetHelperDispatcher = dotNetHelper['_callDispatcher'];
    const dotNetHelperId = dotNetHelper['_id'];
    dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname] ??= {};

    return {
        observersByDotNetObjectId: dotNetHelperDispatcher[dispatcherObserversByDotNetIdPropname],
        id: dotNetHelperId,
    };
}

const invokeInit = (dotNetHelper: DotNet.DotNetObject, container: HTMLElement): void => {
    const contentWidth = container.clientWidth;
    dotNetHelper.invokeMethodAsync("OnContentWidthChange", contentWidth, true);
}

const init = (dotNetHelper: DotNet.DotNetObject, spacerBefore: HTMLElement, spacerAfter: HTMLElement) => {
    const container = spacerBefore.parentElement;
    console.log(container)
    invokeInit(dotNetHelper, container);
    scrollContainer = findClosestScrollContainer(container);
    console.log(scrollContainer);
    const intersectionObserver = new IntersectionObserver(intersectionCallback, {
        root: scrollContainer,
        rootMargin: `50px`,
    });

    intersectionObserver.observe(spacerBefore);
    intersectionObserver.observe(spacerAfter);
    const mutationObserverBefore = createSpacerMutationObserver(spacerBefore);
    const mutationObserverAfter = createSpacerMutationObserver(spacerAfter);

    const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
    observersByDotNetObjectId[id] = {
        intersectionObserver,
        mutationObserverBefore,
        mutationObserverAfter,
    };

    function createSpacerMutationObserver(spacer: Element): MutationObserver {
        const observerOptions = { attributes: true };
        const mutationObserver = new MutationObserver((mutations: MutationRecord[], observer: MutationObserver): void => {
            console.log('AttributesChanged')
            console.log(spacer)
            intersectionObserver.unobserve(spacer);
            intersectionObserver.observe(spacer);
        });

        mutationObserver.observe(spacer, observerOptions);
        return mutationObserver;
    }

    function intersectionCallback(entries: IntersectionObserverEntry[]): void {
        entries.forEach((entry): void => {
            console.log('intersectionCallback')
            if (!entry.isIntersecting) {
                return;
            }
            console.log(entry);
            const containerHeight = (scrollContainer || document.documentElement).clientHeight;
            const scrollTop = (scrollContainer || document.documentElement).scrollTop;
            const scrollHeight = (scrollContainer || document.documentElement).scrollHeight;

            if (entry.target == spacerBefore) {
                console.log('OnSpacerBeforeVisible')
                dotNetHelper.invokeMethodAsync('OnSpacerBeforeVisible', scrollTop, scrollHeight, containerHeight);
            } else if (entry.target == spacerAfter) {
                const style = getComputedStyle(spacerAfter);
                if (parseInt(style.top.replace('px', '')) > 0) {
                    console.log('OnSpacerAfterVisible')
                    dotNetHelper.invokeMethodAsync('OnSpacerAfterVisible', scrollTop, scrollHeight, containerHeight);
                }
            }
        });
    }

    function resizeCallback(entries: ResizeObserverEntry[]): void {
        entries.forEach((entry): void => {
            if (entry.target == scrollContainer) {
                dotNetHelper.invokeMethodAsync("OnContentWidthChange", entry.contentRect.width, false);
            }
        })
    }
}

const scrollTo = (top: number): void => {
    if (!scrollContainer) {
        scrollContainer.scrollTo({
            top: top,
            behavior: 'smooth',
        });
    }
}

const dispose = (dotNetHelper: DotNet.DotNetObject): void => {
    const { observersByDotNetObjectId, id } = getObserversMapEntry(dotNetHelper);
    console.log(observersByDotNetObjectId, id);
    const observers = observersByDotNetObjectId[id];

    if (observers) {
        observers.intersectionObserver.disconnect();
        observers.mutationObserverBefore.disconnect();
        observers.mutationObserverAfter.disconnect();

        dotNetHelper.dispose();
        delete observersByDotNetObjectId[id];
    }
}

export const VirtualWaterfall = {
    init,
    scrollTo,
    dispose,
}

window['VirtualWaterfall'] = VirtualWaterfall